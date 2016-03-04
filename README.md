# barf
Ultralight content management for websites.

A few years ago, I wrote a simple CMS system as a replacement for previous boxed CMSes that were unnecessarily overwrought and kludgy (as opposed to the necessary overwrought kludginess that we just couldn't live without). The secret recipe, like Doofenshmirtz' Grandma's meatloaf, was hate. My hatred fueled my design, a CMS that sat quietly in the corner, serving content fast and then shutting up. The execution of that design was "satisficient", in that it still quietly, speedily serves up content in a production environment by a largish direct sales company that deals in seven different languages.

The interface is a login page and a single admin page, simply a list of content "stubs" with associated HTML or plain-text content. There is support for file storage, but it has been unnecessary, and considering the lightweightedness of this CMS, it will likely assume users of this CMS will have an images store and link to those images rather than serving them up via DB. The database was MS SQL, the data access was ADO.net and a db table/proc creation script that is about 2,000 lines long, but works brilliantly. The front end was knockout and ASP.NET MVC.

My goal with this is to simplify even further. This project will improve upon the old one, getting an update with Angular. I am considering options with the database, but for now, I'll use my existing strategy with modularity that allows the replacement of databases.

Features:
- Store plain text and html. I'm removing support for image storage.
- Multiple language support.
- A simple web interface to add/edit/delete content
- A simple way to retrieve content and display it
- Revision/edits history
