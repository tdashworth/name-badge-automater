# Name Badge Automater

## Summary 

A wizard style application that: 

1. Provide Template
  - a PowerPoint template to download (a single page with 8 badges and first/last name placeholders)
  - upload designed template
  - validate template
    - expect one slide
    - 8 first names and 8 last names templates

2. Provide Names
  - paste (into textbox) a list of people, one per line
  - can be full name or email
    - name: split by first " "
    - email: remove @* and split by "."
  - auto map raw list to first/last names 
  - review and edit these names
    - inline, two textboxes and a delete button
    - add button

3. Download Badges 
  - generate button
  - chunk names into groups of 8
    - copy slide 1 (template)
    - populate with names
    - repeat
    - remaining badges are blanked (no first/last name) (maybe an option?)
  - delete slide 1
  - download file 


  https://www.bootstrapdash.com/product/free-bootstrap-wizard/#product-demo-section
  https://dribbble.com/shots/3051474-Wizard-Ui-Design/attachments/641041
  https://github.com/dotnet/aspnetcore/issues/17730
  https://dev.to/madhust/how-to-publish-blazor-webassembly-application-to-github-pages-using-github-action-54h4
  https://codepen.io/mithicher/pen/wvabGoN
  https://tailwindcomponents.com/component/wizard-steps-bar-with-tailwind-css

  TODO
  - Exception handling 
  - Worker Service
  - Wizard Design 
  - GitHub Pages deploy
  - Application Insights 