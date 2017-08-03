import vuexI18n from 'vuex-i18n';

const languages = [
  'en',
  'ru',
];

const Localization = {
  install(Vue, store) {
    Vue.use(vuexI18n.plugin, store);
    // Load all available localizations
    languages.forEach((language) => {
      // eslint-disable-next-line import/no-dynamic-require
      const translation = require(`../assets/localization/${language}.json`);
      Vue.i18n.add(language, translation);
    });
    // Custom localization function, that can be used for interface localization
    const localize = function $localize(key, options, pluralization) {
      key = `${this.$options.name}_${key}`;
      return Vue.prototype.$t(key, options, pluralization);
    };
    Vue.prototype.$localize = localize;

    // Get all available localizations.
    Vue.getLocalizationList = function getLocalizationList() {
      return languages;
    };

    // Try to detect client language
    let clientLanguage = /(\w+)-?\w*/g.exec(navigator.language)[1];
    // If fonsole is not localized for client's language fallback to english
    if (!languages.includes(clientLanguage)) clientLanguage = 'en';

    Vue.i18n.set(clientLanguage);
  },
};

export default Localization;
