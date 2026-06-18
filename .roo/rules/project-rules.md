# Project Development Rules

> These rules guide ZooCode's behavior when working on this project.

---

## General Project Rules

1. Use .NET 10 with ASP.NET Core and EF Core. Target PostgreSQL 17 via Npgsql provider.

2. Store all uploaded images as binary data in the Media table (bytea column) — do not use filesystem storage.

3. Use Guid primary keys for all entities except SystemSetting (int auto-increment).

4. Implement ASP.NET Core Identity for authentication with a custom UserGroup enum (Author, Admin, Viewer) — do not use string-based roles.

5. The root user is identified by IsRoot=true on AppUser and has unrestricted superuser privileges that bypass all authorization checks.

6. New user registrations default to the Viewer group. Only root can change a user's group.

7. Authors can only edit and delete their own posts. Admin and root can edit/delete any post.

8. Authenticated user comments are immediately visible (IsApproved=true). Guest comments require moderation approval (IsApproved=false initially).

9. Comments support arbitrary nesting depth via self-referencing ParentCommentId. Render nested threads with clear visual indentation.

10. Sanitize all user-generated HTML content (post bodies and comments) to prevent XSS attacks. Use an HTML sanitizer library.

11. Generate URL-friendly slugs from post titles and enforce uniqueness with a database index.

12. The Web API must mirror UI functionality available to Author and Viewer groups and use modern security mechanisms (JWT bearer tokens).

13. Provide Swagger/OpenAPI documentation for all API endpoints.

14. All pages must be fully responsive (desktop, tablet, mobile) using a CSS framework like Bootstrap or Tailwind.

15. Use Docker for PostgreSQL with the configuration specified in the README (postgres:17, network host, named volume postgres_abp_data).

16. Seed the root user and default SystemSettings (BlogTitle, ModerationEnabled) in the initial migration.

17. Use authorization policies (not just role checks) to enforce access control, checking both UserGroup and IsRoot where applicable.

18. Social media and video platform links in posts should be rendered as visually appealing link cards with platform detection (YouTube, Twitter/X, Vimeo, etc.).

19. Include confirmation dialogs for all destructive actions (delete post, delete comment, reject comment, deactivate user).

20. Use async/await throughout all data access and controller actions. Never block on async calls.
