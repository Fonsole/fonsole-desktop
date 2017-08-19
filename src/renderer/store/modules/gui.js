const initialState = {
  page: 'home',
  showTopbar: true,
};

const mutations = {
  setPage(state, page) {
    state.page = page;
  },

  setTopbarVisible(state, visible) {
    state.showTopbar = visible;
  },
};

const getters = {
  getPage: state => () => state.page,
};

export default {
  state: initialState,
  getters,
  // actions,
  mutations,
};
