# Name Badge Automater

This application simply automates the process of entering many names into badge templates. 

**Try it live [here](https://tdashworth.github.io/name-badge-automater/)!**

### Badge template file

Badge templates must be a Powerpoint file with a single slide that will be duplicated based on the number of name and name placeholders replaced. There are two example templates provided but any file meeting the criteria should work. The required placeholders `first_n` and `last_n`, where `n` is the badge index, define where the names will be placed. These can be whatever style, size, location to suite your design. Addtionally, `FIRST_N` and `LAST_N` will transform the given name to uppercase. 

### Parsing names

For ease, names can be pasted into a free textbox then parsed into first and last names. There are a number of formats supported including: 
- FirstName LastName(s)
- LastName(s), FirstName
- FirstName.LastName@email.com
- FirstName LastName(s) <some.email@email.com>
- LastName(s), FirstName <some.email@email.com>

### Generation

Once a valid template file and at least one name is provided, you can generate populated badges. This process, along with everything this app does, is local to your browser so there is no concern your data is leaked. Because of this, the browser will freeze but this shouldn't be for long (depending on the template and number of names). You have the option to tidy up unused name placeholder. 
