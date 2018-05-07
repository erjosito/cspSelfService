copy /Y Web.config.webapp Web.config
git add .
git commit -a -m "new commit"
git push
copy /Y Web.config.local Web.config