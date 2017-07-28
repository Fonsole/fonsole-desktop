import vuexI18n from 'vuex-i18n';

const languages = [
  // First language will be used as default
  'english',

  'russian',
];

const Localization = {
  install(Vue, store) {
    Vue.use(vuexI18n.plugin, store);
    languages.forEach((language) => {
      // eslint-disable-next-line import/no-dynamic-require
      const translation = require(`../assets/localization/fonsole_${language}.json`);
      Vue.i18n.add(language, translation);
    });
    Vue.i18n.set(languages[0]);
  },
};

export default Localization;
