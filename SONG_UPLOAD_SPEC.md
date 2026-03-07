# Song Upload Feature Specification

## Overview
This feature enables automated, asynchronous processing of new music files. When a song is uploaded to Azure Blob Storage, the system will automatically extract its metadata (ID3 tags) and register it in the SQL database without requiring immediate backend intervention.

## Architecture
1. **Storage:** The song is uploaded to the `songs-landing-zone` container in Azure Blob Storage as a ZIP file containing the audio and a metadata file.
2. **Event:** Azure Blob Storage emits a `BlobCreated` event.
3. **Routing:** **Azure Event Grid** (System Topic) captures the event and routes it to a **Broker**.
4. **Translation & Validation (ACL):** The **Broker** (Azure Function) acts as an Anti-Corruption Layer. It validates the ZIP structure and translates the raw storage event into a high-level **Domain Event** (`SongUploadedDomainEvent`).
5. **Messaging:** The domain event is published to an **Azure Service Bus Queue** (for cost optimization).
6. **Processing (The Conductor):** A downstream **Azure Function** (SongProcessor) is triggered by the Service Bus message. It calls the `RegisterSongUseCase` to handle the multi-step workflow.

## The "Extract & Move" Workflow (Use Case)
The `RegisterSongUseCase` coordinates the following steps via Application Layer ports:
1.  **Retrieve:** Open a read stream for the ZIP blob in the landing zone (`ISongStorage`).
2.  **Extract:** Parse the ZIP to get the audio file and metadata (`IZipService`).
3.  **Persist Media:** Upload the extracted audio file to the permanent `music` container (`ISongStorage`).
4.  **Persist Metadata:** Create a new `Song` record in the SQL database (`ISongRepository`).
5.  **Cleanup:** Delete the original ZIP from the landing zone container (`ISongStorage`).

## Architectural Evolution & Trade-offs

### Phase 1: Cost-Optimized MVP (Current)
*   **Infrastructure:** **Service Bus Basic Tier (Queues)**.
*   **Cost:** ~$0/month (Pay-per-operation).
*   **Pattern:** Point-to-point. Only one "Processor" can consume each event.
*   **Benefit:** Demonstrates temporal decoupling and infrastructure isolation without the fixed "Standard Tier" monthly fee.

### Phase 2: Enterprise Fan-out (Future Evolution)
*   **Scenario:** A hypothetical "Recommendations Team" or "Analytics Team" needs to react to the same `SongUploaded` event without interfering with the Music API.
*   **Infrastructure Upgrade:** **Service Bus Standard Tier (Topics)**.
*   **Cost:** ~$10/month (Fixed base charge).
*   **Pattern:** **Fan-out (Pub/Sub)**. Multiple independent Subscriptions (one for each team) receive a copy of every event.
*   **Migration Effort (Zero to Minimal):**
    *   **Application Layer:** **Zero changes.** The Use Case remains decoupled from the messaging medium.
    *   **Broker (Producer):** **Zero code changes.** Update the destination string in the configuration from a Queue name to a Topic name.
    *   **Processor (Consumer):** **Minimal change.** Update the `[ServiceBusTrigger]` attribute in the Azure Function to include the `subscriptionName` parameter.
    *   **Terraform:** Update the SKU to `Standard`, replace the `queue` resource with a `topic` resource, and add an `azurerm_servicebus_subscription` resource.

## Domain Ownership & Organizational Alignment

In a large-scale enterprise, this architecture aligns with clear team boundaries. **Note:** While the following organizational structure is hypothetical for this project, it is included to demonstrate how the architecture supports real-world enterprise scaling:

*   **Bounded Context Ownership:** The **Music Team** owns the "Song Ingestion" Bounded Context. This includes the Landing Zone, the Broker, and the permanent Music Storage.
*   **The Broker as a Service:** The Music Team provides the **Broker** as a shared service to the organization. By doing so, they act as the **Source of Truth** for all song-related events.
*   **The Contract (Domain Event):** The `SongUploadedDomainEvent` is a public contract defined and maintained by the Music Team.
*   **Encapsulation:** This prevents "Infrastructure Leakage," where other teams might otherwise try to listen directly to raw Storage events (Event Grid), which would force them to understand the internal ZIP structure and validation rules.

## Data Integrity & Reliability (The "Reliability Contract")

*   **Implicit Acknowledgement (Broker):** The Broker Function uses the Event Grid Trigger's built-in reliability. If it fails, Event Grid triggers its **24-hour retry policy**.
*   **At-Least-Once Delivery:** To handle potential duplicate signals, the **Processor** is **Idempotent**. It checks if a song already exists before creating a new record.
*   **Chain of Custody:** The original ZIP is only deleted as the **final step** of the `RegisterSongUseCase`.
*   **Dead-Lettering:** If all retries expire, the message is moved to a **Dead-Letter Queue (DLQ)** for manual inspection.

## Implementation Action Plan (Execution Order)

### Phase 1: Core Foundation (Refactoring) [COMPLETE]
1.  **Refactor Ports:** Move `ISongStorage` to `music-streaming-application/ports` and expand its interface (`OpenReadStreamAsync`, `UploadFileAsync`, `DeleteFileAsync`).
2.  **Update Query Handlers:** 
    - Move "URL Generation" logic from the Infrastructure layer to the Query Handlers (e.g., `GetSongsQueryHandler`).
    - Update `SongQueryService_EntityFramework` to return raw data, letting the Application layer handle URI enrichment via `ISongStorage`.
3.  **Update Repository:** Update `SongRepository_EntityFramework` and its interfaces to reflect the new port locations.

### Phase 2: Domain Logic & Workflow
4.  **Define Domain Contracts:** 
    - Create `SongUploadedDomainEvent` in the Application layer.
    - Define the `IZipService` port for extracting audio and metadata from ZIP files.
5.  **Implement Use Case:** 
    - Build `RegisterSongUseCase` to coordinate the "Landing Zone -> Move -> SQL -> Cleanup" dance.
    - Implement **Idempotency checks** (e.g., checking if the audio file already exists in the destination container).

### Phase 3: Infrastructure Adapters
6.  **Storage Implementation:** Update the `AzureBlobStorageAdapter` to implement the new `ISongStorage` methods.
7.  **Utility Implementation:** Implement `ZipService` using `System.IO.Compression`.

### Phase 4: Cloud Infrastructure (Terraform)
8.  **Provision Storage:** Add the `songs-landing-zone` container.
9.  **Provision Messaging:** Add the Service Bus Namespace (Basic), the `song-uploaded-queue`, and the Event Grid System Topic/Subscription (with path filtering).
10. **Provision Compute:** Add the Linux Function App (Consumption Plan) with **System-Assigned Managed Identity**.
11. **Configure Permissions (RBAC):** Grant the Function App's identity `Storage Blob Data Contributor` and `Service Bus Data Sender/Receiver` roles.

### Phase 5: The "Head" (Azure Functions)
12. **Project Setup:** Create a new .NET 8 Azure Function project in the `music_streaming_service` solution.
13. **Dependency Injection:** Configure DI to inject the Application layer Use Cases and Infrastructure adapters into the Functions.
14. **Broker Function:** Implement the `EventGridTrigger` function (Validate ZIP -> Map to Domain Event -> Publish to Queue).
15. **Processor Function:** Implement the `ServiceBusTrigger` function (Deserialize -> Call `RegisterSongUseCase`).

### Phase 6: Validation
16. **End-to-End Test:** Perform a real upload to the Landing Zone and verify the full chain: Event Grid -> Broker -> Queue -> Processor -> SQL -> Cleanup.
