const initialState = {
  page: 'home',
};

const mutations = {
  setPage(state, page) {
    state.page = page;
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
