CSCI 211 - Client Side Programming
12/10/2023
Task 2
Cody Cusey

This app is designed as a single user interface for adding, editing, viewing, and managing a physical music media library.
There is a fairly basic homepage to help direct the user and interactions.
The Album Info page allows user to view the entire list of albums, loaded in from the albumsdb.json file.
From the same page, the user has the option to edit an album and make changes to certain criteria such as: Artist, Title, Label, Track Count, or Year Released.
The backend albumsdb.json file is dynamically changed using the axios framework to get, put, and post data from the database based on user interactions.
The Add Album page does exactly what it describes. There is input validation on every field to ensure full object data as well as unique IDs.
Upon successful addition, there's a simple confirmation page to give user feedback on submission.
There is default scss implementation as well as some basic, but custom CSS for specific elements used (like error messages).
