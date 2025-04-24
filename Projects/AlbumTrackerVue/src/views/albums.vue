<!-- Viewing Album Information page, lists all items in the backend albumsdb.json file, loops through each item based on id to gather all.
     Some CSS used here to aid in appearance. -->

<template>
    <div class="content-container">
      <div class="columns">
        <div class="column is-8">
          <div class="section content-title-group">
            <h2 class="title"><u>Albums List</u></h2>
            <ul>
              <!-- The looping of all albums in albumsdb.json based on id -->
              <li v-for="album in albums" :key="album.id">
                <div class="card">
                  <div class="card-content">
                    <div class="content">
                      <!-- Several div tags to display the information being gathered during the loop, one object per loop with several variables -->
                      <div :key="album.id" class="name">
                        <b>Album ID: </b>{{ album.id }} 
                      </div>
                      <div>
                        <b>Format: </b>{{ album.format }}
                      </div>
                      <div>
                        <b>Genre: </b>{{ album.genre }}
                      </div>
                      <div>
                        <b>Artist: </b>{{ album.artist }}
                      </div>
                      <div>
                        <b>Title: </b>{{ album.title }}
                      </div>
                      <div>
                        <b>Label: </b>{{ album.label }}
                      </div>
                      <div>
                        <b>Duration: </b>{{ album.duration }} mins
                      </div>
                      <div>
                        <b>Cost: </b>${{ album.cost }}
                      </div>
                      <div>
                        <b>Track Count: </b>{{ album.trackcount }}
                      </div>
                      <div>
                        <b>Year Released: </b>{{ album.released }}
                      </div>
                    </div>
                  </div>
                  <footer class="card-footer">
                    <!-- Button to select for editing, and the calls needed to pull in the album information based on the id parameter -->
                    <router-link
                      tag="button"
                      class="link card-footer-item"
                      :to="{ name: 'AlbumDetail', params: { id: album.id } }"
                    >
                      <i class="fas fa-check"></i>
                      <span>Select Album for Editing</span>
                    </router-link>
                  </footer>
                </div><br>
              </li>
            </ul>
          </div>
          <div class="notification is-info" v-show="message">{{ message }}</div>
        </div>
      </div>
    </div>
  </template>
  
  <!-- Beginning of our javascript portion of the code (the meat) so to speak -->

  <!-- Importing of our dataservice "client" the help aid with handling the REST services used by axios -->
  <script>
  import { dataService } from '../shared';

  // The default data to be exported based on the relative data to Albums path, will be the entire album object and message field (to be used however)  
  export default {
    name: 'Albums',
    data() {
      return {
        albums: [],
        message: '',
      };
    },

    // Beginning of the methods to be loaded during the lifecycle
    async created() {
      await this.loadAlbums();
    },

    // Call to load in all albums into an array
    methods: {
      async loadAlbums() {
        this.albums = [];
        this.message = 'Getting the albums, please be patient...';
  
        this.albums = await dataService.getAlbums();
  
        this.message = '';
      },
    },
  };
  </script>
  