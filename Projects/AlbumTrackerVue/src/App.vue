<!-- Links to the routes needed to fulfill data service requests, as well as importing the scss file need to style the pages -->

<template>
  <div id="app">
    <nav>
      <router-link to="/">Home</router-link> |
      <router-link to="/albums">Album Info</router-link> |
      <router-link to="/add">Add Album</router-link> |
      <router-link to="/about">About</router-link>
    </nav>
    <router-view/>
  </div>
</template>

<style lang="scss">
@import '@/design/index.scss';
</style>
