const { execSync } = require('child_process');

// This command compares local and remote trees and outputs their names
const buffer = execSync('git diff-tree -r --name-only --no-commit-id ORIG_HEAD HEAD');
// Result of execSync is buffer, so convert it to string
const changedList = buffer.toString();
// Files in output are divided with \n, so split them and get array of changed files
const changedFiles = changedList.trim().split('\n');

/**
 * Check if git pull has changed file(s) that fulfill specified mask.
 *
 * @param {RegExp} mask - File mask regular expression
 * @param {function|string} action - A function that will be executed if condition is fulfilled
 *                                   Also can be a shell command string, that will be executed.
 */
module.exports = (mask, action) => {
  if (changedFiles.some(file => mask.test(file))) {
    if (typeof action === 'string') {
      // If action is a string it's a shell command
      // Just execute it.
      console.log(`husky > hook > ${action}`);
      // Pipe all command's stdio to stdio of this process
      execSync(action, { stdio: 'inherit' });
    } else if (typeof action === 'function') {
      // Action is just a function, call it
      action();
    } else {
      throw new TypeError('action argument should be a string or function');
    }
  }
};
