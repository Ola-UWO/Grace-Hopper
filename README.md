Sprint 4 changes: 
- Integrated Microsoft authentication (MSAL) with proper iOS keychain entitlements and Azure AD app registration to support secure sign-in.

- Implemented OneDrive access through Microsoft Graph v5, including token handoff from MSAL and a custom Graph authentication provider.

- Added the ability to load and display .docx manager logs directly from a designated OneDrive folder, sorted and time-formatted for clarity.

- Enabled file downloads and system-level opening via Launcher.OpenAsync for viewing OneDrive log documents on device.

- Refined UI behavior, including loading indicators, button state changes based on sign-in status, empty-state messaging, and a fully scrollable logs list.

Sprint 3 changes:

- Added some funtionality to managerlogs page

- Implemented a funtional opening checklist

- Implemented a funtional closing checklist

- Implemented web scraping in order to get an up-to-date list of events

- Cleaned up the database implementation and added loose RLS

- Replaced the hard-coded Manager Logs list with a dynamic CollectionView that loads the most recent entries from the `manager_logs` table. Added the `ManagerLog` model and database retrieval methods to support live data.


Sprint 2 changes:

-Made all necessary screens as of this version of the app (some new features are in discussion)

-Added Shell navigation with a view model that allows pages to bind to a NavigateCommand
and specify what page to navigate to 🤓 

-Added a massive backend (Business logic + Database layers) to show database connectivity 👨🏽‍💻

-Made values displayed on screens bind to an observable collection to display data 
