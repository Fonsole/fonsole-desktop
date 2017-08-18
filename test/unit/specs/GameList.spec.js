import Vue from 'vue';
import store from '@/store';
import Localization from '@/localization';
import GameList from '@/components/Workshop/GameList';
// @ifdef ELECTRON
import { shell } from 'electron';
// @endif

Vue.use(Localization, store);

describe('GameList.vue', () => {
  const vm = new Vue({
    el: document.createElement('div'),
    render: h => h(GameList),
    store,
  }).$mount();

  describe('should correctly render game list', () => {
    store.state.workshop = [
      {
        path: 'd:/dev/fonsole/games/game1',
        name: 'Game 1',
        tags: [
          'strategy',
          'action',
        ],
        players: [3, 6],
        workshopId: 1234567,
      },
      {
        path: 'd:/dev/fonsole/games/game2',
        name: 'Game 2',
        tags: [
          'strategy',
          'action',
        ],
        players: [3, 6],
        workshopId: undefined,
      },
    ];

    const entries = vm.$el.querySelectorAll('.game-list-entry');

    it('Game count should equal to rendered elements count', () => {
      expect(entries).to.have.lengthOf(store.state.workshop.length);
    });

    store.state.workshop.forEach((game) => {
      it(`should create list element for ${game.name}`, () => {
        expect(Array.prototype.some.call(entries, el => el.textContent.trim() === game.name.trim()))
          .to.be.true;
      });
    });
  });

  it('should render 3 doc links', async () => {
    const links = vm.$el.querySelectorAll('.create-game-info > a');
    expect(links).to.have.lengthOf(3);
  });

  it('should open "createGame" page when clicked on button', () => {
    vm.$el.querySelector('.create-game-button').click();
    expect(store.state.gui.page).to.equal('createGame');
  });

  describe('doc links should be clickable', () => {
    const links = vm.$el.querySelectorAll('.create-game-info > a');
    const spyElectron = sinon.stub(shell, 'openExternal');
    // const spyWeb = sinon.stub(window, 'open');
    links.forEach((link) => {
      it(`Link "${link.textContent}" should be clickable`, () => {
        link.click();
        expect(spyElectron).to.have.been.calledWith(`${link.href}`);
      });
    });
  });
});
