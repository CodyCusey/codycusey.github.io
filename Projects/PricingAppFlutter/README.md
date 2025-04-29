# Flutter Pricing Application

My Flutter based Single Page Pricing Application made for American Chemet Corporation.

Challenge: Design a single code based single page application to validate pricing requests for purchases by American Chemet Corporation.

Context: Purchasing policies at American Chemet Corporation state that pricing is at the sellers option. What this means for the Purchasing Department, is that suppliers can place requests for pricing virtually whenever. It is our obligation to fulfill these pricing requests as soon as possible. The shortcomings in the system is that these requests cannot be validated unless on a computer. The pricing app fixes that!

Action: The user can select from various different deployment platforms, including but not limited to: IOS mobile devices and tablets, Android mobile devices and tablets, Windows based Application, Mac OS Application, and Web Application. Upon launching, the user is greeted with a welcome screen requesting the user to input information to be validated. By selecting a Supplier from the dropdown style list and attaching the corresponding PO Number and/or Line Item number, the user can validate pricing requests at the touch of a button.
There are two layers of validation utlized to ensure pricing only occurs once per PO Line Item. The first validation is the Supplier / PO Number validation, which is validated by pulling a .csv file of all bookings in and using the manually entered combination to ensure correct combination. The second layer of validation is via a Microsoft SQL Server Database. All actualy pricing information is stored in this database and is used to validate for invoicing. The single page nature of the application allows the user to quickly change request variables without the need to reload pages between each request.

Result: I successfully completed my pricing application within the required deadline. I learned about the .DART language and widgets within its' framework. I learned how to read in data from mulitple sources, including .csv files and databases. These skills are essential to any application development or programming in general. I was able to perform all of the utility and functionality i had desired, but could be improved upon even more before live deployment.

Reflection: I was in need of a project for my Independent Study course and after chatting with my supervisor, he gave me the idea to persue a mobile pricing app to aid the American Chemet Corporation Purchasing Department in their pricing request fulfillment obligations from wherever they happent to be. It was fun to apply my learnings to an application that I get to use in my real work.

## Welcome Screen

![image](https://github.com/CodyCusey/codycusey.github.io/blob/bf117f678c7883b80931b850383d3940169bb1c3/Projects/PricingAppFlutter/assets/Screenshot%202025-04-28%20211152.png)

## Supplier Select Dropdown

![image](https://github.com/CodyCusey/codycusey.github.io/blob/bf117f678c7883b80931b850383d3940169bb1c3/Projects/PricingAppFlutter/assets/Screenshot%202025-04-28%20211202.png)

## Supplier / PO Validation OK

![image](https://github.com/CodyCusey/codycusey.github.io/blob/bf117f678c7883b80931b850383d3940169bb1c3/Projects/PricingAppFlutter/assets/Screenshot%202025-04-28%20211234.png)

## Supplier / PO Validation OK + Pricing Request Result - Unpriced

![image](https://github.com/CodyCusey/codycusey.github.io/blob/bf117f678c7883b80931b850383d3940169bb1c3/Projects/PricingAppFlutter/assets/Screenshot%202025-04-28%20211908.png)

## Supplier / PO Validation OK + Pricing Request Result - Priced

![image](https://github.com/CodyCusey/codycusey.github.io/blob/bf117f678c7883b80931b850383d3940169bb1c3/Projects/PricingAppFlutter/assets/Screenshot%202025-04-28%20211923.png)

## Supplier / PO Validation BAD + Pricing Request Result - Unpriced

![image](https://github.com/CodyCusey/codycusey.github.io/blob/bf117f678c7883b80931b850383d3940169bb1c3/Projects/PricingAppFlutter/assets/Screenshot%202025-04-28%20211934.png)