This asset solve 2 major problems:

- Sometime you really need to have a folder shared between multiple unity projects. 
It may be a shared graph folder, or libraries of generic scripts, or even a Framework.
But there is no native solution in unity to do so.

The common solution is to create symbolic link via a bash or python script.
But by doing so, you can't push your external file to a git repository!
You have to create multiple git and do complexe stuff to add them together.

With this tool, I offer you an easly way to create jonction between an external folder and your unity project.
These jonctions can be pushed in a git, or in unity collab.



- When you have an external framework shared between multiple project,
the biggest problem we encounter is the difficulty to differentiate between the files coming from the framework,
and those from unity.

These often led to big organisation problems if you unfortunatly delete a picture from the framework inside one project,
and 2 week later on the other project, you discover that the picture you were using is gone !

This tool allow you to always know in what type of external framework a file is. In the project panel,
or even on a gameObject, the little indicator <=> or * indicate if the object you seek is inside an external folder.