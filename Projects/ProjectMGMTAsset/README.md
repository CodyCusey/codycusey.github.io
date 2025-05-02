# ACC Asset Logging System

My participation in designing, configuring, and programming an asset logging and reporting system.

Challenge: Normalize Data and migrate away from user managed spreadsheets. Implement web based data entry system, backend database to store, front end reporting and UI.

Context: For years, spreadsheets have rules and fooled the data landscape at American Chemet Corporation. By implementing an asset logging and reporting system, we will be able to use and record all producion related entries for each asset defined. By the clever use of stored procedures, functional logic, and dynamic SQL, this system can quickly and easily record asset utilization into a database, querey and manipulate as desired, and finally reported back to a defined user group. It's just the tip of the data landscape iceberg for American Chemet Corporation.

Action: Using several different technology tools and platforms, I assisted in creating and designing a Microsoft SQL Database backend to store our user input asset logs. From there I worked to create several linking relational tables to allow the corresponding user input fields to logically match row / column configuration. Once the data was logged, the final piece was to have a "public" report available. Using SQL Server Reporting Services (SSRS) Report Builder, I was able to easily call my nested store procedure to dynamically generate
the fields to be placed in the report table.

Result: I successfully completed my contirbutions to the system and project within project deadline. I was able to collect asset data from production operators (users) and store in a managable database. Furthermore I was able to create a frontend reporting service that can be only accessible to the designated user groups.

Reflection: American Chemet Corporation has been plagued by technology deficits for as long as I have been a team member. By leaning into new data management systems and normalizing complex datasets, American Chemet Corporation will propel itself into the spot of world leader in not one, but a family metal based chemicals and powders.
