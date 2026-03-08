# Song Upload Feature Specification

## Overview
This feature enables automated, asynchronous processing of new music files. When a song is uploaded to Azure Blob Storage, the system will automatically extract its metadata (ID3 tags) and register it in the SQL database without requiring immediate backend intervention.

## Architecture
1. **Storage:** The song is uploaded to the `songs-landing-zone` container in Azure Blob Storage as an audio file (`.mp3`, `.wav`).
2. **Event:** Azure Blob Storage emits a `BlobCreated` event.
3. **Routing:** **Azure Event Grid** (System Topic) captures the event and routes it to a **Broker**.
4. **Translation & Validation (ACL):** The **Broker** (Azure Function) acts as an Anti-Corruption Layer. It validates the file type and translates the raw storage event into a high-level **Domain Event** (`SongUploadedDomainEvent`).
5. **Messaging:** The domain event is published to an **Azure Service Bus Queue** (for cost optimization).
6. **Processing (The Conductor):** A downstream **Azure Function** (SongProcessor) is triggered by the Service Bus message. It calls the `RegisterSongUseCase` to handle the multi-step workflow.

## The "Extract & Move" Workflow (Use Case)
The `RegisterSongUseCase` coordinates the following steps via Application Layer ports:
1.  **Retrieve Metadata:** Open a read stream for the blob in the landing zone and extract ID3 tags (`IMetadataService`).
2.  **Normalize Database:** Ensure the **Artist** and **Album** exist in the database, creating them if necessary.
3.  **Generate Deterministic Name:** Calculate a safe, collision-resistant filename based on metadata: `{uploaderId}-{album-slug}-{title-slug}.{ext}`.
4.  **Persist Media:** Stream the audio file from the landing zone to the permanent `music` container (`ISongStorage`).
5.  **Persist Metadata:** Create a new `Song` record linked to the correct `AlbumId` (`ISongRepository`).
6.  **Cleanup:** Delete the original file from the landing zone container (`ISongStorage`).

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

## Data Integrity & Reliability (The "Reliability Contract")

*   **Idempotency:** The **Processor** checks if a song already exists (by Title and Album) before creating a new record.
*   **Deterministic Naming:** Filenames are generated from metadata, ensuring that retries overwrite the same file rather than creating duplicates or orphaned media.
*   **Database Normalization:** All songs are strictly linked to an `Album`, which is linked to an `Artist`, ensuring high data integrity and rich query capabilities.
*   **Chain of Custody:** The original file is only deleted as the **final step** of the `RegisterSongUseCase`.

## Implementation Action Plan (Execution Order)

### Phase 1: Core Foundation (Refactoring) [COMPLETE]
1.  **Refactor Ports:** Move `ISongStorage` to `music-streaming-application/ports`.
2.  **Update Query Handlers:** Handle URI enrichment in the Application layer.
3.  **Update Repository:** Update `SongRepository_EntityFramework` interfaces.

### Phase 2: Domain Logic & Normalization [COMPLETE]
4.  **Define Domain Contracts:** 
    - Create `SongUploadedDomainEvent` in the Application layer.
    - Define the `IMetadataService` port for ID3 extraction (replaces `IZipService`).
5.  **Database Normalization:** 
    - Introduced **Artists** and **Albums** tables.
    - Updated Domain and Persistence models to use a hierarchical structure (Artist -> Album -> Song).
    - Removed redundant IDs from domain objects to enforce Clean Architecture.
6.  **Implement Use Case:** 
    - Build `RegisterSongUseCase` with deterministic naming logic and JIT artist/album provisioning.
    - Verified logic with AAA unit tests in `music-streaming-application.tests`.

### Phase 3: Infrastructure Adapters [CURRENT]
7.  **Metadata Implementation:** Implement `MetadataService_TagLib` using TagLibSharp for stream-based extraction.
8.  **Storage Implementation:** Update `AzureBlobStorageAdapter` to implement full read/write/delete capabilities.

### Phase 4: Cloud Infrastructure (Terraform)
9.  **Provision Storage:** Add the `songs-landing-zone` container.
10. **Provision Messaging:** Add the Service Bus Namespace (Basic) and Event Grid System Topic.
11. **Provision Compute:** Add the Linux Function App.

### Phase 5: The "Head" (Azure Functions)
12. **Broker Function:** Implement `EventGridTrigger` (Validation -> Domain Event -> Queue).
13. **Processor Function:** Implement `ServiceBusTrigger` (Trigger `RegisterSongUseCase`).

### Phase 6: Validation
14. **End-to-End Test:** Verify the full chain from landing zone upload to SQL registration.
