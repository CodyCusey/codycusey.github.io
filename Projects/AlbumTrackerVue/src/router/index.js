import Vue from 'vue'
import VueRouter from 'vue-router'
import HomeView from '../views/HomeView.vue'

Vue.use(VueRouter)

const parseProps = r => ({ id: parseInt(r.params.id) });

const routes = [

  // Route for the path to the Home page.
  {
    path: '/',
    name: 'home',
    component: HomeView
  },

  // Route for the path to the Albums List page.
  {
    path: '/albums',
    name: 'Albums',
    component: () =>
      import(/* webpackChunkName: "bundle.albums" */ '../views/albums.vue'),
  },

  // Route for the path to the Album Edit page.
  {
    path: '/albums/:id',
    name: 'AlbumDetail',
    // props: true,
    props: parseProps,
    component: () =>
      import(/* webpackChunkName: "bundle.albums" */ '../views/album-detail.vue'),
  },

  // Route for the path to the Album Addition page.
  {
    path: '/add',
    name: 'Add',
    // props: true,
    props: parseProps,
    component: () =>
      import(/* webpackChunkName: "bundle.albums" */ '../views/add-album.vue'),
  },

  // Route for the path to the About page.
  {
    path: '/about',
    name: 'about',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ '../views/AboutView.vue')
  },

  // Route for the path to the Album Addition Confirmation page.
  {
    path: '/addconfirm',
    name: 'addconfirm',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "addconfirm" */ '../views/addconfirm.vue')
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router
