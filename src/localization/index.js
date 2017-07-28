const Localization = {
  install(Vue) {
    Vue.setupLocalization = function setupLocalization() {
      const translationEnglish = require('../assets/localization/fonsole_english.json');
      const translationRussian = require('../assets/localization/fonsole_russian.json');

      Vue.i18n.add('en', translationEnglish);
      Vue.i18n.add('ru', translationRussian);

      Vue.i18n.set('en');
    };
  },
};

export default Localization;
