import Vue from 'vue';
import store from '@/store';
import Localization from '@/localization';
import Topbar from '@/components/topbar/Topbar';

Vue.use(Localization, store);

describe('Topbar.vue', () => {
  const vm = new Vue({
    el: document.createElement('div'),
    render: h => h(Topbar),
    store,
  }).$mount();

  it('should render 5 buttons', async () => {
    // TODO Use ids
    expect(vm.$el.querySelectorAll('ul > li > div > a').length).to.have.lengthOf(5);
  });
});
