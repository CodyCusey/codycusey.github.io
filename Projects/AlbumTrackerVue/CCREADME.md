# Album Tracker

My Album Tracking Application I created as a single page application (SPA) using Vue.

Challenge: Design a web application that allows the user to add, delete, edit, and view albums in their collection.

Context: For the final project in my Client-Side Programming class I needed to create some type of web application. I created a web app that allows the user to manage their physical media library in a digital format. The single page configuration allows for a quick and responsive UI for adding new albums, editing information on previous entries, deleting entries, or simply viewing their collection.

Action: Using Visual Studio Code and Vue, I created a Home view that clearly directs the user to the options available. From the Album Info selection, the user is presented with a list of all items in their collection. This is where the user can peruse their media library, and make a selection for editing. Once in the editing view, the user can alter certain attributes to update the selected album to better represent the version in their collection. From the Add Album selection, the user can enter in information to be added as a new entry into the collection. The AlbumID was left as a manual entry, with uniqueness validated intentionally. Certain cataloguing systems having specific naming conventions and this flexibility allows user to curate to their organizational specifications. This app is designed to only run on the client side, so there is technically no back-end development. The data is collected and stored in JSON formatting in a local file build into the app. The use of an API handles all the CRUD operations to make use of the JSON data and data field entry.

Result: I successfully completed my application within deadline criteria and learned how to use Vue to create a single-page application. I learned how to implement efficient code validation and dynamically alter the web page based on user interaction. I also learned how to utilize an API to manipulate data from user entered data fields using CRUD operations.

Reflection: I was desperately in need of a simple, fast, and robust cataloguing system for all of my physical media collections. Using the single page application (SPA) approach, I was able to create a more responsive and user-friendly web experience.


## Vue CLI commands to run application in Visual Studio Code

To launch Node.js backend:
```
npm run backend
```

To run Vue.js frontend:
```
npm run serve
```

## Welcome page

![image](https://github.com/CodyCusey/codycusey.github.io/blob/f7550c36f7da14ffd0d660681e323f3de374ec69/Projects/AlbumTrackerVue/src/assets/Screenshot%202025-04-22%20125919.png)

## Album List page

![image](https://github.com/CodyCusey/codycusey.github.io/blob/f7550c36f7da14ffd0d660681e323f3de374ec69/Projects/AlbumTrackerVue/src/assets/Screenshot%202025-04-22%20125933.png)

## Edit Album page

![image](https://github.com/CodyCusey/codycusey.github.io/blob/f7550c36f7da14ffd0d660681e323f3de374ec69/Projects/AlbumTrackerVue/src/assets/Screenshot%202025-04-22%20125940.png)

## Add Album page

![image](https://github.com/CodyCusey/codycusey.github.io/blob/f7550c36f7da14ffd0d660681e323f3de374ec69/Projects/AlbumTrackerVue/src/assets/Screenshot%202025-04-22%20130111.png)

## Validation and Confirmation pages

![image](https://github.com/CodyCusey/codycusey.github.io/blob/f7550c36f7da14ffd0d660681e323f3de374ec69/Projects/AlbumTrackerVue/src/assets/Screenshot%202025-04-22%20130123.png)
![image](https://github.com/CodyCusey/codycusey.github.io/blob/f7550c36f7da14ffd0d660681e323f3de374ec69/Projects/AlbumTrackerVue/src/assets/Screenshot%202025-04-22%20130134.png)

## Displaying New Album (from Add Album)

![image](https://github.com/CodyCusey/codycusey.github.io/blob/f7550c36f7da14ffd0d660681e323f3de374ec69/Projects/AlbumTrackerVue/src/assets/Screenshot%202025-04-22%20130143.png)
