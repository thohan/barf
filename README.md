# barf
Ultralight content management for websites.

A few years ago, I wrote a simple content system as a replacement for previous boxed CMSes that were unnecessarily expensive, overwrought, and kludgy. The secret ingredient, like Doofenshmirtz' Grandma's meatloaf, was hate. Hatred for having to do weird, anti-pattern, anti-decency kinds of things to get our websites to work with the CMSes. My hatred fueled the design, a content delivery system that sat quietly in the corner, serving content fast and without drama. The execution of that design was sufficient for our needs: It still quietly, speedily serves content in a production environment for our largish direct sales company that maintains content in seven languages for a multitude of countries, and has been doing so for over two years. The best thing that one could hear about a CMS that is working is nothing at all.

The interface is a login page and a single admin page, simply a list of content "stubs" with associated HTML or plain-text content. There is also a page for language administration. There is support for file storage, but it has been unused, and considering the lightweightedness of this CMS, it will likely assume users of this CMS will have an images/files store and link to those images/files via html rather than serving them up via DB (if this is not the case in your situation, this project would be inadequate for you). The database is MS SQL, the data access is ADO.net and a db table/proc creation script that is about 2,000 lines long. The front end is knockout and ASP.NET MVC.

My goal with this project is to further simplify and improve upon the old design. It will be a from-scratch rewrite, getting an update with Angular for the front end. I am considering various database options.

Features:
- Store and retrieve plain text and html. I'm removing support for image storage.
- Multiple language support.
- A simple web interface to add/edit/delete content (ckeditor is the current choice)
- A simple way to retrieve content and display it on a website
- Revision/edits history and diff panel using jsdiff (https://github.com/kpdecker/jsdiff)
