const initialState = {
  page: 'home',
};

const mutations = {
  setPage(state, page) {
    state.page = page;
  },
};

export default {
  state: initialState,
  // getters,
  // actions,
  mutations,
};
