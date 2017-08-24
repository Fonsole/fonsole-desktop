import Ajv from 'ajv';
import schema from '@/assets/.fonsole.schema.yml';

const gameIndex = new Ajv().compile(schema);

/**
 * Checks game index file (.fonsole.yml) for matching schema
 *
 * @export
 * @param {Object} content Parsed .fonsole.yml file
 */
// eslint-disable-next-line import/prefer-default-export
export function verifyGameIndex(content) {
  return gameIndex(content);
}
