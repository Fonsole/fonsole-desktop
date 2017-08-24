/**
 * @file This component implements functions to get and set values of settings.
 *       This component is designed to be extended with some template.
 */
export default {
  name: 'Setting',
  props: {
    setting: {
      type: String,
      required: true,
    },
  },
  computed: {
    value: {
      get() {
        return this.$store.state.settings[this.setting];
      },
      set(newValue) {
        // Get current value
        const oldValue = this.value;

        // Synchronously update local settings state and asynchronously save it to file system.
        this.$store.dispatch('setValue', {
          key: this.setting,
          value: newValue,
        });

        // Emit 'change' event
        this.$emit('change', newValue, oldValue);
      },
    },
  },
};
