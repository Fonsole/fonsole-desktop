import Vue from 'vue';
import store from '@/store';
import Localization from '@/localization';
import Topbar from '@/components/Topbar/Topbar';

Vue.use(Localization, store);

describe('Topbar.vue', () => {
  const vm = new Vue({
    el: document.createElement('div'),
    render: h => h(Topbar),
    store,
  }).$mount();

  it('should render 6 buttons', async () => {
    // TODO Use ids
    const buttons = vm.$el.querySelectorAll('ul > li > div > a:not(#room)');
    expect(buttons).to.have.lengthOf(6);
  });

  describe('buttons should work', () => {
    const buttons = vm.$el.querySelectorAll('ul > li > div > a:not(#room)');
    buttons.forEach((button) => {
      it(`Button "${button.textContent}" should open page "${button.id}"`, () => {
        button.click();
        expect(store.state.gui.page).to.equal(button.id);
      });
    });
  });
});
