import Vue from 'vue';
import Router from 'vue-router';
import Game from './Game';

Vue.use(Router);

export default new Router({
  mode: 'history',
  routes: [
    { path: '/game/:game', component: Game },
  ],
});
