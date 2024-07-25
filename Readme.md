# PresidioGenspark-Capstone-Project
## News Application

## Overview
The News Application provides users with the latest news articles from various categories, leveraging external news APIs for live updates. The app includes features for user authentication via OAuth, article management, commenting, and administrative functionalities.

## Features

### 1. User Features

#### 1.1. Authentication
- **OAuth Login:** Users can log in using their existing accounts from providers such as Google or Facebook.
  - **Flow:** Users click "Login with Google," are redirected to the provider's authorization page, and upon granting permissions, are redirected back to the app.
  - **Data Handled:** OAuth tokens and user information (e.g., name, email).
- **Account Management:**
  - **Registration:** Users can create an account using OAuth or manually (if implemented).
  - **Login/Logout:** Users can log in using their credentials or OAuth and log out from the application.

#### 1.2. News Feed
- **View Articles:** Users can browse and view the latest news articles fetched from external APIs.
  - **Article Details:** Includes the title, content, summary, image, and timestamp.
  - **Ranking:** View Top 3 articles of a particular category.
- **Article Customization:**
  - **Saved Articles:** Users can save articles for later reading.
  - **Personalized Feeds:** Users can customize their news feed based on interests and preferred categories.

#### 1.3. Social Interaction
- **Comments:**
  - **Add Comment:** Users can add live comments to articles.
  - **Edit/Delete Comment:** Users can delete their own comments.
- **Article Sharing:**
  - **Share Articles:** Users can share articles on social media platforms.

#### 1.4. Accessibility and Usability
- **Dark Mode:** Users can switch to a dark mode for better readability.
- **Font Size Adjustment:** Users can adjust the font size for better accessibility.

### 2. Admin Features

#### 2.1. User Management
- **Manage Users:** Admins can view and manage user accounts.
  - **Roles and Permissions:** Admins can assign roles and set permissions for different users.

#### 2.2. Content Management
- **Category Management:**
  - **Manage Categories:** Admins can add, edit, or delete categories for better organization of articles.
  - **Approve/Reject Articles:** Admins can review and approve, edit or reject articles before they appear in the feed.

#### 2.3. Analytics and Reporting
- **Article Performance:** Admins can analyze article performance based on shares, and comments.

### 3. System Architecture

#### 3.1. Presentation Layer
- **UI/UX Components:** Interfaces for users and admins, including article views, comment sections, and profile management.

#### 3.2. Business Logic Layer
- **Core Functionalities:** Implementations for user authentication, article management, and commenting.

#### 3.3. Data Access Layer
- **Database Interactions:** CRUD operations for users, comments, and categories.

#### 3.4. Integration Layer
- **External APIs:** Interfaces with external news APIs for fetching live news data.

### 4. Database Schema
- **User:** userId (PK), name, email, role, oauthId, oauthToken
- **Article:** articleId (PK), title, content, summary, addedAt, originalurl, imgurl, createdAt, Impscore, sharecount
- **Comment:** commentId (PK), articleId (FK), userId (FK), content, timestamp
- **Category:** categoryId (PK), name, description
- **ArticleCategory:** articleId (CK), categoryId (CK)
- **SavedArticle:** savedarticleId (PK), userId (FK), articleId (FK), savedAt timestamp
