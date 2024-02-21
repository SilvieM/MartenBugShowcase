Minimal Project using Marten 6 to showcase the rebuild stuck issue.
Issue happens when using AllDocumentsEnforceOptimisticConcurrency.

Run: 
Make a postgres db, adjust connection string in Program.cs to match.
Start. Call addThing once, then call rebuild endpoint. 
After addThing you will see it builds the projection. Rebuild gets stuck and the projection is gone.
If you comment out AllDocumentsEnforceOptimisticConcurrency then rebuild does not get stuck.
Make sure to clean and rebuild every time because sometimes it does not fully apply the changes.