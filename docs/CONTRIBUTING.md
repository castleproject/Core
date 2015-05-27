The documentation is written in Markdown.

[Atom](https://atom.io/) is a good editor for GitHub Flavored Markdown because it has a preview window.

To keep the syntax consistent we use [markdownlint](https://github.com/mivok/markdownlint), please run it if you are changing
a significant amount of documentation.

Markdownlint can be installed by running.

```
gem install mdl
```

Running the linter is as easy as:

```
cd doc
mdl --style=markdownlint.rb .
```