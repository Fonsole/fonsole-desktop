import _ from 'lodash';
import { screen } from 'electron';

function isValidNumber(n) {
  return Number.isSafeInteger(n) && n > 0;
}

export default {
  validate: bounds => bounds === null || (
    isValidNumber(bounds.x) &&
    isValidNumber(bounds.y) &&
    isValidNumber(bounds.width) &&
    isValidNumber(bounds.height)
  ),
  default() {
    const screenBounds = screen.getPrimaryDisplay().bounds;
    return _.mapValues(screenBounds, value => value * 0.75);
  },
};
