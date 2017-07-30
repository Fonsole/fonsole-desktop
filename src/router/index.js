import Vue from 'vue';
import Router from 'vue-router';
import Game from './Game';
import Settings from '../components/settings/Settings';

Vue.use(Router);

export default new Router({
  mode: 'history',
  routes: [
    { path: '/settings', component: Settings },
    { path: '/game/:game', component: Game },
  ],
});
