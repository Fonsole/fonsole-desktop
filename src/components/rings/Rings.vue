<template lang="html">
  <div id="rings">
    <canvas id="particles" ref="particles"></canvas>
  </div>
</template>

<script>
  const Proton = window.Proton;

  export default {
    name: 'rings',
    data() {
      return {
        initialRadius: 10,
        colours: ['#69D2E7', '#A7DBD8', '#E0E4CC', '#F38630', '#FA6900', '#FF4E50', '#F9D423'],
      };
    },
    methods: {
      random(m, n) {
        if (Array.isArray(m)) {
          return m[Math.floor(Math.random() * m.length)];
        }
        const mi = parseInt(m, 10);
        const ni = parseInt(n, 10);
        return Math.floor(Math.random() * ((ni - mi) + 1)) + mi;
      },
    },
    mounted() {
      const canvas = this.$refs.particles;
      canvas.width = window.innerWidth;
      canvas.height = window.innerHeight;

      const context = canvas.getContext('2d');

      const proton = new Proton();
      const emitter = new Proton.Emitter();
      const scaleFactor = Math.sqrt((window.innerWidth ** 2) + (window.innerHeight ** 2))
        / this.initialRadius;
      emitter.rate = new Proton.Rate(new Proton.Span(1));
      emitter.addInitialize(new Proton.Radius(this.initialRadius));
      emitter.addInitialize(new Proton.Life(20));
      emitter.addBehaviour(new Proton.Color('random'));
      emitter.addBehaviour(new Proton.Scale(1, scaleFactor));
      emitter.p.x = canvas.width;
      emitter.p.y = canvas.height;
      emitter.emit();
      proton.addEmitter(emitter);
      const renderer = new Proton.Renderer('canvas', proton, canvas);
      renderer.onProtonUpdate = function onProtonUpdate() {
        context.fillStyle = 'rgba(0, 0, 0, 0.1)';
        context.fillRect(0, 0, canvas.width, canvas.height);
      };
      renderer.start();

      function tick() {
        requestAnimationFrame(tick);
        proton.update();
      }
      tick();

      function resize() {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;

        emitter.p.x = canvas.width;
        emitter.p.y = canvas.height;
      }

      window.addEventListener('resize', resize, false);
    },
  };
</script>

<style lang="scss">
  #rings {
    position:absolute;
  }
</style>
