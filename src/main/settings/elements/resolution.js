import { screen } from 'electron';

export default {
  verify: value => typeof value === 'string' && /^\d{1,5}x\d{1,5}$/.test(value),
  default: () => {
    const display = screen.getPrimaryDisplay();
    const bounds = display.bounds;
    return `${bounds.width}x${bounds.height}`;
  },
};
