# barf
Ultralight content management for websites.

A few years ago, I wrote a simple content system as a replacement for previous boxed CMSes that were unnecessarily expensive, overwrought, and kludgy. The secret ingredient, like Doofenshmirtz' Grandma's meatloaf, was hate. Hatred for having to do weird, anti-pattern, anti-decency kinds of things to get our websites to work with the CMSes. My hatred fueled the design, a content delivery system that sits quietly in the corner, serving content fast and without drama. The execution of this design is sufficient for our needs: It still quietly, speedily serves content in a production environment for our largish direct sales company that maintains content in seven languages for a multitude of countries, and has been doing so since 2013.

The interface is a login page and a single admin page, simply a list of content "stubs" with associated HTML or plain-text content. There is also a page for language administration. There is support for file storage, but it has been unused so far. The database is MS SQL, the data access is ADO.net and a db table/proc creation script that is about 2,000 lines long. The front end is knockout and ASP.NET MVC.

My goal with this project is to further simplify and improve upon the old design. It will be a rewrite that will hopefully take advantage of a light ORM tool such as Dapper to replace the old-timey DAO code. It would be nice to get into some Angular2/Typescript as well.

Features:
- Store and retrieve plain text and html. I'll continue image storage support for now.
- Multiple language support.
- A simple web interface to add/edit/delete content (ckeditor is the current choice of WYSIWYG windows)
- A simple way to retrieve content and display it on a website, via Razor helper or javascript object.
- Revision history and diff panel using jsdiff (https://github.com/kpdecker/jsdiff)
