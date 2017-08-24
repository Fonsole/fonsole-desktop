import Vue from 'vue';
import Vuex from 'vuex';
import * as actions from './actions';
import * as getters from './getters';
import gui from './modules/gui';
import networking from './modules/networking';
import gameLibrary from './modules/gameLibrary';
import settings from './modules/settings';
// @ifdef ELECTRON
import workshop from './modules/workshop';
import workspace from './modules/workspace';
// @endif

Vue.use(Vuex);

export default new Vuex.Store({
  actions,
  getters,
  modules: {
    gui,
    networking,
    gameLibrary,
    settings,
// @ifdef ELECTRON
    workshop,
    workspace,
// @endif
  },
});
