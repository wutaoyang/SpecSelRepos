Instructions to set up the site upon deployment:

Once the website is published to a live server, the necessary database tables for users and spec sel 
results do not exist, unless you have selected to apply migrations on publish. It is assumed that you 
have assigned the correct connection strings when publishing to a server and that the program can 
connect to an existing database. 

If this is the case, you should click on 'register' and enter details for the main administrator of the 
website. The username should be an email address. When you click register, you should be logged in, or if 
migrations are yet to be applied to create the required tables, the webpage should offer to apply the 
migrations. Allow it  to do so. 

CREATE FIRST ADMIN USER:
At this point you are logged into the website. 
Append the website address with /Account/CreateAdmin?email=admin@uea.ac.uk (replace admin@uea.ac.uk with 
the username entered for the admin user). A message should appear saying that the user is now the first 
admin. Log out and back into the admin account. You will now see an 'Admin' link in the menu bar at the 
top of the page. Clicking on this allows new roles to be created and for roles to be added or removed 
from users.
This process creates the admin and user roles which cannot be deleted. Users can hold multiple roles. An
admin may manage roles of users but cannot edit the spec sel results database. A user can edit the 
database but cannot manage roles. A user with both roles may do either. A person who registers but has no
assigned roles may view the database but cannot make any site modifications.

Regards
mre16utu@uea.ac.uk