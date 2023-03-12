# PhotoOrganizer
This is my PhotoVersor desktop application.
* I have a lot of scanned old photos.
Do you have a similar situation? I know there are a bunch of nice free or paid solutions available, but I thought creating my own would meet my expectations entirely.

* My intention was to create a comfortable tool, especially for my family and myself. This WPF app can deal with my old pictures. There is a navigation bar where the photos will be listed, and then you can easily select one or more and open them in a separate tab.

* The metadata of each photo is reflected in the editor view, where you can change anything you can see. Have you found a picture that you don't remember? Just put it on the shelf, and if something similar comes up with the same topic or about an event, then you can complete editing it.

* If you find a lot of pictures from the same event with the same people in the same place, just select all related items and easily copy the metadata from the open one to others in the list.

* If you forgot something and don't want the changes to be applied, no problem! Just save it and persist the metadata information later in one click.

Here are some screenshots of my app because a picture can say more than words :)

# WELCOME SCREEN
![00](https://user-images.githubusercontent.com/22032902/223402457-50c1d203-ecc1-4c84-983b-0aba13c13f61.JPG)

# Import/loading pictures
You can import/load one picture or entire content of directories (from subdirectories as well). If your actual project is not empty, than it asks, would you extend or overwrite: 
![01](https://user-images.githubusercontent.com/22032902/223403516-720bec4d-ff6d-490a-a968-82977723faaf.JPG)
![02](https://user-images.githubusercontent.com/22032902/223403519-66303584-6e19-45d7-816a-289d7ff5f57d.JPG)

# Layout
You can see the editor tabs in the middle. Navigation bar on the bottom with the 'Shelve'.
You can change Title, Description, Adding People, Location information Taken Date. Here it is, how it looks like:
![03](https://user-images.githubusercontent.com/22032902/223403522-a6753298-3a51-4759-ae36-0740ce12b663.JPG)

# Map
You can easily set a location with direct typing of coordinates or set it from the map. And you can also name your favourite location if you need to use them later:
![04](https://user-images.githubusercontent.com/22032902/223403526-b92c29d5-d3a7-40bd-8b74-c56af7177b46.JPG)

# Easy-Copy: Faster progress
Next to the of the textboxes there is an icon which can copy the data to the selected items from the navigation bar.
Or there is an ultimate button to copy all data if it is easier for you.
![05](https://user-images.githubusercontent.com/22032902/223403529-4ffb5e9c-d6e0-4dbc-993b-fe68175ec547.JPG)

# People
You can add/remove/edit people. They will be saved if you need to tag them on other pictures. You can add people easier with a built in scripting mechanism improved by me:
![06](https://user-images.githubusercontent.com/22032902/223403535-2dbbef11-fd2e-497e-84e2-4cc0240ce535.JPG)

# Menu
Here you can see the menu options

## File

![File](https://user-images.githubusercontent.com/22032902/223403538-dc9f485f-dcfb-4866-af47-73eb7637e738.JPG)

* Read entire library or just add singe photos.
* All saved items can be saved by one click
* You can create Albums [this feature is not done]
* You can save all open tabs
* Exit of course. In this case application will ask to save open items.
There are 3 phases regarding to file handling:
1. Editing - unsaved items /asterix on tab title/
2. Saved - saved only in the database /yellow status in the Navigation/
3. Metadata Saved - metadata information is backed into image /green status in the Navigation/

## Data

![08](https://user-images.githubusercontent.com/22032902/223403540-3cef3f13-e0c3-4b74-865d-3eadca327078.JPG)

* Create backup - create backup file from your database [it is a beta functionality]
* Erase all data.. - It will empty your database. Before doing so it will ask about making a backup.

## Edit

![09](https://user-images.githubusercontent.com/22032902/223403541-1dc1f17b-b15d-4b6d-b1c6-9cc7f770e93d.JPG)

* Locations - You can edit the formerly saved locations [beta]

## Window

![10](https://user-images.githubusercontent.com/22032902/223403543-25dc4589-b2a8-420f-a1a0-308645cca902.JPG)

* Close Open Tabs - Same in English :) It will ask about saving before doing it's job

## Options

![11](https://user-images.githubusercontent.com/22032902/223403544-835c9205-80fb-4c27-ba5e-9a0ea1deece7.JPG)

* Settings - You can change the language and page size
1. All text will be affected by language change (menu, buttons, toopltips, hints, descriptions). English and Hungarian is available.
2. Page sizes is a performance optimization [beta]. It can influence the memory consumption.

![13](https://user-images.githubusercontent.com/22032902/223403549-ee6f97b9-1ff5-4a80-8391-adbc9a8215db.JPG)

## Help

![12](https://user-images.githubusercontent.com/22032902/223403547-c0199e67-8473-4571-919e-9fa9fadc855b.JPG)

* View help - Hints, tipps and detailed information about functionality

![14](https://user-images.githubusercontent.com/22032902/223403552-571611a9-bc3d-4dd6-9e2a-fea1efaea8fb.JPG)
![15](https://user-images.githubusercontent.com/22032902/223403555-e0d6612f-207e-4860-ae7c-a14f36c27e9d.JPG)
![16](https://user-images.githubusercontent.com/22032902/223403558-352c6cad-50a8-44c5-9d43-98c117e642c2.JPG)
