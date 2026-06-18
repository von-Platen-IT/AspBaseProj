# Database Schema — Single Source of Truth

> This file defines the database schema for the project.
> ZooCode uses this as the authoritative reference when generating or modifying database-related code.
> **Edit this file to reflect schema changes** — the AI agent will update code accordingly.

**Technology:** .NET 10 Fullstack (ASP.NET Core, PostgreSQL, EF Core)
**Last Updated:** 2026-06-18 18:30

---

## Entities

### AppUser

Represents a registered user of the blog platform. Users are assigned to a group (Author, Admin, Viewer) and the root user has unrestricted superuser privileges.

#### Fields

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| `Id` | `Guid` | No | Primary key |
| `UserName` | `string` | No | Unique username for login |
| `Email` | `string` | No | Unique email address |
| `EmailConfirmed` | `bool` | No | Whether the email has been confirmed |
| `PasswordHash` | `string` | No | Hashed password |
| `Group` | `UserGroup` | No | Enum: Author, Admin, Viewer |
| `IsRoot` | `bool` | No | True only for the root superuser account |
| `DisplayName` | `string` | Yes | Optional display name shown publicly |
| `AvatarImageId` | `Guid?` | Yes | FK to Media for user avatar |
| `CreatedAt` | `DateTime` | No | Account creation timestamp |
| `UpdatedAt` | `DateTime?` | Yes | Last profile update timestamp |
| `IsActive` | `bool` | No | Whether the account is active and can log in |

#### Relationships

- **One To Many** `Post`: A user (Author) can create many posts
- **One To Many** `Comment`: A user can write many comments
- **One To Many** `Media`: A user can upload many media files
- **One To One** `Media`: A user may have one avatar image

---

### Post

A blog post authored by a registered user. Contains HTML content, supports embedded images and social media/video links.

#### Fields

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| `Id` | `Guid` | No | Primary key |
| `Title` | `string` | No | Post title, max 200 characters |
| `Slug` | `string` | No | URL-friendly unique identifier for the post |
| `ContentHtml` | `string` | No | Full HTML body content from rich-text editor |
| `Excerpt` | `string` | Yes | Optional short summary for list views |
| `AuthorId` | `Guid` | No | FK to AppUser who authored the post |
| `IsPublished` | `bool` | No | Whether the post is publicly visible |
| `PublishedAt` | `DateTime?` | Yes | When the post was first published |
| `CreatedAt` | `DateTime` | No | Creation timestamp |
| `UpdatedAt` | `DateTime?` | Yes | Last edit timestamp |
| `ViewCount` | `int` | No | Number of times the post has been viewed |
| `UpvoteCount` | `int` | No | Total number of upvotes |
| `DownvoteCount` | `int` | No | Total number of downvotes |

#### Relationships

- **Many To One** `AppUser`: Each post belongs to one author
- **One To Many** `Comment`: A post can have many comments
- **One To Many** `Media`: A post can have multiple embedded images
- **One To Many** `UserVote`: A post can have many votes

---

### Comment

A comment on a blog post or a reply to another comment (nested discussions). Authenticated users' comments are immediately visible; guest comments require moderation approval.

#### Fields

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| `Id` | `Guid` | No | Primary key |
| `PostId` | `Guid` | No | FK to the Post being commented on |
| `ParentCommentId` | `Guid?` | Yes | FK to parent Comment for nested replies; null for top-level comments |
| `UserId` | `Guid?` | Yes | FK to AppUser if commenter is authenticated; null for guests |
| `GuestName` | `string` | Yes | Name provided by guest commenter; null for authenticated users |
| `GuestEmail` | `string` | Yes | Email provided by guest commenter; null for authenticated users |
| `Content` | `string` | No | Comment text content |
| `IsApproved` | `bool` | No | True if visible publicly. Authenticated user comments are auto-approved; guest comments start false |
| `IsRejected` | `bool` | No | True if a moderator rejected this comment |
| `CreatedAt` | `DateTime` | No | Comment creation timestamp |
| `ApprovedAt` | `DateTime?` | Yes | When the comment was approved by a moderator |
| `ApprovedById` | `Guid?` | Yes | FK to AppUser (Admin/root) who approved the comment |
| `UpvoteCount` | `int` | No | Total number of upvotes |
| `DownvoteCount` | `int` | No | Total number of downvotes |

#### Relationships

- **Many To One** `Post`: Each comment belongs to one post
- **Many To One** `Comment`: A comment can have a parent comment (for nesting)
- **One To Many** `Comment`: A comment can have many child replies
- **Many To One** `AppUser`: Optional: the authenticated user who wrote the comment
- **Many To One** `AppUser`: Optional: the admin/root who approved the comment
- **One To Many** `UserVote`: A comment can have many votes

---

### Media

Stores uploaded images (post embeds, avatars) directly in the database as binary data. Supports content-type tracking and is linked to the uploading user and optionally a post.

#### Fields

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| `Id` | `Guid` | No | Primary key |
| `FileName` | `string` | No | Original file name |
| `ContentType` | `string` | No | MIME type (e.g., image/png, image/jpeg) |
| `Data` | `byte[]` | No | Binary content of the file |
| `FileSize` | `long` | No | Size in bytes |
| `UploadedById` | `Guid` | No | FK to AppUser who uploaded the file |
| `PostId` | `Guid?` | Yes | FK to Post if the media is embedded in a specific post; null for avatars or general uploads |
| `CreatedAt` | `DateTime` | No | Upload timestamp |

#### Relationships

- **Many To One** `AppUser`: Each media file was uploaded by one user
- **Many To One** `Post`: Optional: media may belong to a specific post

---

### SystemSetting

Key-value store for configurable system settings such as blog title, moderation process toggles, and other administrative options.

#### Fields

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| `Id` | `int` | No | Primary key (auto-increment) |
| `Key` | `string` | No | Unique setting key (e.g., 'BlogTitle', 'ModerationEnabled') |
| `Value` | `string` | Yes | Setting value as string |
| `Description` | `string` | Yes | Human-readable description of the setting |
| `UpdatedAt` | `DateTime?` | Yes | Last modification timestamp |
| `UpdatedById` | `Guid?` | Yes | FK to AppUser who last changed the setting |

#### Relationships

- **Many To One** `AppUser`: Optional: the user who last updated the setting

---

### SocialLink

Represents a social media or video platform link embedded in a post, rendered as a visually appealing link card.

#### Fields

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| `Id` | `Guid` | No | Primary key |
| `PostId` | `Guid` | No | FK to the Post containing this link |
| `Url` | `string` | No | The full URL to the social media or video resource |
| `Platform` | `string` | Yes | Detected or specified platform name (e.g., YouTube, Twitter, Vimeo) |
| `Title` | `string` | Yes | Optional display title for the link card |
| `DisplayOrder` | `int` | No | Ordering of links within the post |
| `CreatedAt` | `DateTime` | No | Creation timestamp |

#### Relationships

- **Many To One** `Post`: Each social link belongs to one post

---

## Indexes

- `unnamed`: Unique index on username
- `unnamed`: Unique index on email
- `unnamed`: Unique index on slug for URL routing
- `unnamed`: Index for querying posts by author
- `unnamed`: Index for chronological listing
- `unnamed`: Composite index for retrieving comment threads per post
- `unnamed`: Index for moderation queue queries
- `unnamed`: Index for retrieving media for a post
- `unnamed`: Unique index on setting key

## Additional Notes

The root user is a special flag (IsRoot=true) on AppUser rather than a separate role, granting unrestricted superuser privileges. Guest comments are stored with UserId=null and require explicit moderator approval before becoming publicly visible. Media files are stored as binary data in PostgreSQL rather than on the filesystem. The Comment entity uses self-referencing ParentCommentId for arbitrarily deep nesting. SocialLink entities allow visually appealing embeds for external social/video platforms within posts.

---

## How to Modify This Schema

1. Edit the entity definitions above (add/remove/modify fields, relationships, etc.)
2. ZooCode will detect changes to this file and update:
   - Database migration files
   - Model/entity classes
   - API endpoints that interact with changed entities
   - Repository/data access layer code
