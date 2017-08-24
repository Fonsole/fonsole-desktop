// eslint-disable-next-line import/prefer-default-export
export function stringToResolution(s) {
  return s.split('x').map(Number);
}
