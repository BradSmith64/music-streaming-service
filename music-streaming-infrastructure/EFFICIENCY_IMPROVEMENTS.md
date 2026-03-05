# EF Core Efficiency Improvements

This document tracks implemented and proposed performance optimizations for the `music-streaming-infrastructure` project.

## 1. Implemented Improvements

### Avoided Client-Side Evaluation (SAS Tokens)
- **Location**: `SongQueryService_EntityFramework.cs`
- **Issue**: Calling `_storage.GetFileUri()` inside a `.Select()` projection caused EF Core to perform client-side evaluation, fetching all data into memory before generating tokens sequentially.
- **Fix**: Data is now fetched using a projection to an anonymous type first, and then mapped to the `SongMetadata` domain object in memory.

### Removed Redundant Database Round-trips
- **Location**: `SongRepository_EntityFramework.cs`
- **Issue**: `LikeSongAsync` and `UnlikeSongAsync` were performing an explicit `.Any()` check before their main operation, adding an extra `SELECT 1` query to every request.
- **Fix**: 
    - `LikeSongAsync` now attempts the insert directly and handles exceptions only on failure.
    - `UnlikeSongAsync` fetches the record first and only checks for song existence if the record is missing.
- **Async Optimization**: All remaining checks were switched to `AnyAsync()` to prevent thread blocking.

### Corrected Documentation
- **Location**: `SongQueryService_EntityFramework.cs`
- **Change**: Removed an erroneous comment about N+1 queries. Modern EF Core correctly translates `.Count` and `.Any()` navigation properties into SQL subqueries when used in a projection.

### Explicit SongId Index
- **Location**: `AppDbContext.cs`
- **Change**: Formally defined the index for `SongId` on the `Likes` table.
- **Benefit**: While EF Core handles this by convention for foreign keys, making it explicit ensures it's visible in code and provides reliable lookup performance.

---

## 2. Proposed / Future Improvements

### Memory Optimization: Direct Projection & No-Tracking
- **Target**: `SongRepository_EntityFramework.GetSongByIdAsync`
- **Current State**: Fetches a `Persistence.Song` entity (tracked by EF) and then manually maps it to a `Domain.Song` object.
- **Proposal**: 
    - Use `.AsNoTracking()` to reduce change-tracker overhead.
    - Project directly into the `Domain.Song` object within the `.Select()` to avoid creating the intermediate persistence entity.
- **Benefit**: Reduced memory allocation and faster execution by bypassing the EF change tracker and reducing object counts.

### Database Integrity: Composite Unique Index
- **Target**: `Likes` Table
- **Status**: **On Hold** (Decided not to implement for now as `UserId` is not yet a formal entity).
- **Proposal**: Add a composite unique index on `(SongId, UserId)` once the user system is formalized.
- **Benefit**: Ensures a user can only like a song once (at the schema level) and further optimizes user-specific lookups.
