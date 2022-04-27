cat .git/config  # note <github-uri>
rm -rf .git
git init
git branch -M main
git add .
git commit -m "Initial commit"
git remote add origin git@github.com:netnr/test.git
git push -u --force origin main
