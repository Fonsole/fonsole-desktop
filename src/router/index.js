import Vue from 'vue';
import Router from 'vue-router';
import Game from './Game';
import Home from '../components/home/Home';
import Games from '../components/games/Games';
import Shop from '../components/shop/Shop';
import Community from '../components/community/Community';
import Settings from '../components/settings/Settings';

Vue.use(Router);

export default new Router({
  mode: 'history',
  routes: [
    { path: '/home', component: Home },
    { path: '/games', component: Games },
    { path: '/shop', component: Shop },
    { path: '/community', component: Community },
    { path: '/settings', component: Settings },
    { path: '/game/:game', component: Game },
  ],
});
