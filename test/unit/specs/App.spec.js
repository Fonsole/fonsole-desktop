import Vue from 'vue';
import store from '@/store';
import Localization from '@/localization';
import App from '@/App';

Vue.use(Localization, store);

describe('App.vue', () => {
  const vm = new Vue({
    el: document.createElement('div'),
    render: h => h(App),
    store,
  }).$mount();

  it('should render 6 buttons', async () => {
    // TODO Use ids
    const buttons = vm.$el.querySelectorAll('ul > li > div > a:not(#room)');
    expect(buttons).to.have.lengthOf(6);
  });

  describe('should render different components based on vuex gui plugin state', () => {
    Object.keys(App.components).forEach((component) => {
      it(`Component "${component}" should be rendered when gui state is "${component}"`, async () => {
        store.commit('setPage', component);
        await Vue.nextTick();
        expect(vm.$children[0].page).to.equal(store.state.gui.page);
      });
    });
  });
});
