import vuexI18n from 'vuex-i18n';

const languages = [
  // First language will be used as default
  'en',

  'ru',
];

const Localization = {
  install(Vue, store) {
    Vue.use(vuexI18n.plugin, store);
    languages.forEach((language) => {
      // eslint-disable-next-line import/no-dynamic-require
      const translation = require(`../assets/localization/${language}.json`);
      Vue.i18n.add(language, translation);
    });
    Vue.i18n.set(/(\w+)-?\w*/g.exec(navigator.language)[1]);
    const localize = function $localize(key, options, pluralization) {
      key = `${this.$options.name}_${key}`;

      return Vue.prototype.$t(key, options, pluralization);
    };

    Vue.prototype.$localize = localize;
    Vue.getLocalizationList = function getLocalizationList() {
      return languages;
    };
  },
};

export default Localization;
