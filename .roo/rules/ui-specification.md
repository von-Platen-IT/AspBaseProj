# UI Specification — Single Source of Truth

> This file defines the user interface specification for the project.
> ZooCode uses this as the authoritative reference when generating or modifying UI code.
> **Edit this file to reflect UI changes** — the AI agent will update code accordingly.

**Technology:** .NET 10 Fullstack (ASP.NET Core, PostgreSQL, EF Core)
**Last Updated:** 2026-06-17 13:01

---

## Pages / Views

### Home / Post List

**Route:** `/`

Chronological list of all published blog posts with excerpts, author names, and publication dates. Accessible to all visitors.

#### Components

| Component | Type | Description |
|-----------|------|-------------|
| `PostCard` | `list` | Card showing post title, excerpt, author, date, and thumbnail if available |
| `Pagination` | `navigation` | Page navigation for post list |
| `BlogHeader` | `navigation` | Site header with blog title from SystemSettings, login/register or user menu |
| `SearchBar` | `form` | Search posts by title or content |

#### User Actions

- Browse posts
- Click post to read full content
- Search posts
- Navigate pages
- Log in / Register

---

### Post Detail

**Route:** `/posts/{slug}`

Full blog post view with rendered HTML content, embedded images, social link cards, and the comment section with nested replies.

#### Components

| Component | Type | Description |
|-----------|------|-------------|
| `PostContent` | `detail` | Renders full HTML content with embedded images and social link cards |
| `PostMeta` | `detail` | Shows author, publication date, view count, and edit/delete buttons if permitted |
| `CommentSection` | `list` | Threaded/nested comment display with reply buttons |
| `CommentForm` | `form` | Form to add a comment; for guests includes name/email fields and shows moderation notice |
| `ReplyForm` | `form` | Inline reply form for responding to a specific comment |

#### User Actions

- Read post
- View embedded images
- Follow social links
- Add comment (authenticated or guest)
- Reply to existing comment
- Edit/delete post (if owner or admin/root)

---

### Login

**Route:** `/account/login`

Login page for registered users.

#### Components

| Component | Type | Description |
|-----------|------|-------------|
| `LoginForm` | `form` | Username/email and password fields with submit button |
| `RegisterLink` | `navigation` | Link to registration page |

#### User Actions

- Enter credentials
- Submit login
- Navigate to register

---

### Register

**Route:** `/account/register`

Registration page for new users. New users are assigned the Viewer group by default.

#### Components

| Component | Type | Description |
|-----------|------|-------------|
| `RegisterForm` | `form` | Username, email, password, and confirm password fields |

#### User Actions

- Fill registration form
- Submit registration

---

### Post Editor

**Route:** `/posts/create and /posts/{slug}/edit`

Rich-text HTML editor for creating and editing blog posts. Available to Author, Admin, and root users. Authors can only edit their own posts.

#### Components

| Component | Type | Description |
|-----------|------|-------------|
| `TitleInput` | `form` | Text input for post title; auto-generates slug |
| `HtmlEditor` | `form` | WYSIWYG HTML editor with formatting toolbar |
| `ImageUploader` | `form` | Upload images that are stored in the database and embedded in content |
| `SocialLinkEditor` | `form` | Add social media/video links with platform detection for visual link cards |
| `ExcerptInput` | `form` | Optional excerpt/summary field |
| `PublishToggle` | `form` | Toggle to publish or save as draft |
| `SaveButton` | `form` | Save or update post |

#### User Actions

- Write and format post content
- Upload and embed images
- Add social/video links
- Set excerpt
- Publish or save draft
- Update existing post

---

### User Profile

**Route:** `/account/profile`

Logged-in user's profile page showing their information, group membership, and their posts.

#### Components

| Component | Type | Description |
|-----------|------|-------------|
| `ProfileInfo` | `detail` | Display username, email, group, display name, avatar |
| `ProfileEditForm` | `form` | Edit display name, email, and avatar |
| `MyPostsList` | `list` | List of posts authored by the current user with edit/delete actions |

#### User Actions

- View profile
- Edit profile info
- Upload avatar
- View own posts
- Edit/delete own posts

---

### Admin Dashboard

**Route:** `/admin`

Administrative overview page for Admin and root users. Provides access to user management, moderation queue, content management, and system settings.

#### Components

| Component | Type | Description |
|-----------|------|-------------|
| `AdminNav` | `navigation` | Navigation cards/links to admin sub-sections |
| `StatsOverview` | `chart` | Summary statistics: total users, posts, pending comments |
| `PendingCommentsWidget` | `list` | Quick view of pending guest comments awaiting moderation |

#### User Actions

- View admin overview
- Navigate to sub-sections
- Quick moderate pending comments

---

### Moderation Queue

**Route:** `/admin/moderation`

List of all guest comments awaiting approval. Admin and root can approve or reject individually or in bulk.

#### Components

| Component | Type | Description |
|-----------|------|-------------|
| `PendingCommentsTable` | `table` | Table of unapproved guest comments with guest name, content, post title, and timestamp |
| `BulkActions` | `form` | Select-all checkbox and bulk approve/reject buttons |
| `CommentActions` | `form` | Per-comment approve and reject buttons |

#### User Actions

- Review pending comments
- Approve individual comment
- Reject individual comment
- Bulk approve selected
- Bulk reject selected

---

### User Management

**Route:** `/admin/users`

Root-only page for managing all users and their group assignments. Root can change any user's group, activate/deactivate accounts, and view user activity.

#### Components

| Component | Type | Description |
|-----------|------|-------------|
| `UsersTable` | `table` | Table of all users with username, email, group, status, and creation date |
| `GroupSelector` | `form` | Dropdown to change a user's group (Author/Admin/Viewer) |
| `UserActions` | `form` | Activate/deactivate toggle, edit user details |

#### User Actions

- View all users
- Change user group
- Activate/deactivate user
- Edit user details

---

### Content Management

**Route:** `/admin/content`

Admin/root page for managing all posts and comments across all authors. Allows editing or deleting any content.

#### Components

| Component | Type | Description |
|-----------|------|-------------|
| `AllPostsTable` | `table` | Table of all posts with title, author, status, and dates |
| `AllCommentsTable` | `table` | Table of all comments with content, author/guest, post, and approval status |
| `ContentActions` | `form` | Edit and delete buttons for posts and comments |

#### User Actions

- View all posts
- Edit any post
- Delete any post
- View all comments
- Delete any comment
- Remove inappropriate content

---

### System Settings

**Route:** `/admin/settings`

Root-only page for configuring system-wide settings such as blog title and moderation process options.

#### Components

| Component | Type | Description |
|-----------|------|-------------|
| `SettingsForm` | `form` | Editable form for all SystemSetting key-value pairs |
| `SaveSettingsButton` | `form` | Save settings changes |

#### User Actions

- Edit blog title
- Toggle moderation process
- Save system settings

---

### API Documentation

**Route:** `/api/docs`

Swagger/OpenAPI documentation page for the Web API. Available to Author and Viewer groups via API tokens.

#### Components

| Component | Type | Description |
|-----------|------|-------------|
| `SwaggerUI` | `detail` | Interactive API documentation with endpoint testing |
| `AuthTokenInput` | `form` | Input for API authentication token |

#### User Actions

- Browse API endpoints
- Test API calls
- Authenticate with token

---

## Navigation Structure

- **structure:** Top navigation bar (BlogHeader) is present on all pages with: Blog title (links to Home), Search bar, and user menu (Login/Register for guests; Profile, Logout for authenticated users; Admin Dashboard link for Admin/root). On mobile, the navigation collapses into a hamburger menu. The Admin Dashboard serves as a hub with navigation cards to Moderation Queue, User Management, Content Management, and System Settings. Post Detail pages have contextual edit/delete buttons for authorized users.

## Theme & Styling

- **description:** Clean, modern, responsive design using a CSS framework (e.g., Bootstrap or Tailwind). Blog posts use readable typography with comfortable line lengths. Social link cards have visually appealing embeds with platform-specific styling. Comment threads use indentation and visual connectors for nesting depth. All pages are fully responsive across desktop, tablet, and mobile with adaptive layouts and touch-friendly controls.

---

## How to Modify This Specification

1. Edit the page/component definitions above
2. ZooCode will detect changes to this file and update:
   - UI component files (widgets, pages, views)
   - Routing configuration
   - Navigation components
   - Style/theme files
