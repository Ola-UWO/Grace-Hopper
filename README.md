Sprint 3 changes:
-Implemented a funtional opening checklist

-Implemented a funtional closing checklist

-Implemented web scraping in order to get an up-to-date list of events

-Cleaned up the database implementation and added loose RLS

- Replaced the hard-coded Manager Logs list with a dynamic CollectionView that loads the most recent entries from the `manager_logs` table. Added the `ManagerLog` model and database retrieval methods to support live data.


Sprint 2 changes:

-Made all necessary screens as of this version of the app (some new features are in discussion)

-Added Shell navigation with a view model that allows pages to bind to a NavigateCommand
and specify what page to navigate to 🤓 

-Added a massive backend (Business logic + Database layers) to show database connectivity 👨🏽‍💻

-Made values displayed on screens bind to an observable collection to display data 
