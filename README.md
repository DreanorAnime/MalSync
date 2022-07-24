# MalSync
Updates your Kitsu list with all recent entries from your MyAnimeList.

Login credentials are for Kitsu only.

MyAnimeList updates are fetched from https://myanimelist.net/animelist/{username}/load.json?offset=0&status=7&order=5

Not all Titles will work, detection is very barebones for now.

# Usage
```
Parameters: KitsuUsername MalUsername KitsuPassword NumberOfEntriesToUpdate

ex. "MalSync.exe KitsuUsername MalUsername MyKitsuPassword123 10"
```