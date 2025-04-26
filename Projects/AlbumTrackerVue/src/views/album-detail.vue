<!-- Edit Album Page, creates a web based user input form, with differing field types, cancel and save buttons, as well as some dynamic vue items
     There is CSS helping the page layout look appealing.-->

<template>
  <div>
    <div class="section content-title-group">
      <h2 class="title">Edit Album</h2>
      <div class="card">
        <header class="card-header">

          <!-- Dynamic header that displays the album title, as bound to the vue model, the value passed in utilizes interpolation syntax {{ }} -->
          <h2 class="card-header-title"><b>{{ albumName }}</b></h2>
        </header>
        <div class="card-content">
          <div class="content">

             <!-- Basic textbox type input fields and binding to the vue model "dotted" reference style, this will be the field used for unique id validation -->
            <div class="field">
              <label class="label" for="id"><b>Album Id: </b></label>
              <label class="input" name="id" readonly>{{ album.id }}</label><br>
            </div>

            <!-- Select dropdown list, with a default and disabled value, binded to the vue model "dotted" style, used to aid the user in operations -->
            <div class="field">
                <label for="format">
                    <b>Format: </b><br>
                        <div class="select">            
                            <select id="format" v-model="album.format">
                                <option disabled value="">Please Select a Format</option>
                                <option>Vinyl</option>
                                <option>Cassette</option>
                                <option>8-Track</option>
                                <option>MiniDisc</option>
                                <option>CD</option>
                                <option>SACD</option>
                                <option>DAT</option>
                                <option>Reel-to-Reel</option>
                            </select>
                        </div>
                    </label>
              </div>

            <!-- Several more basic textbox type input fields and binding to the vue model "dotted" reference style. -->
            <div class="field">
              <label class="label" for="genre"><b>Genre: </b></label>
              <input class="input" name="genre" v-model="album.genre" />
            </div>
            <div class="field">
              <label class="label" for="artist"><b>Artist: </b></label>
              <input class="input" name="artist" v-model="album.artist" />
            </div>
            <div class="field">
              <label class="label" for="title"><b>Title: </b></label>
              <input class="input" name="title" v-model="album.title" />
            </div>
            <div class="field">
              <label class="label" for="label"><b>Label: </b></label>
              <input class="input" name="label" v-model="album.label" />
            </div>
            <div class="field">
              <label class="label" for="duration"><b>Duration: (mins)</b></label>
              <input class="input" name="duration" v-model="album.duration" />
            </div>
            <div class="field">
              <label class="label" for="cost"><b>Cost: (USD)</b></label>
              <input class="input" name="cost" v-model="album.cost" />
            </div>
            <div class="field">
              <label class="label" for="trackcount"><b>Track Count: </b></label>
              <input class="input" name="trackcount" v-model="album.trackcount" />
            </div>
            <div class="field">
              <label class="label" for="released"><b>Year Released: </b></label>
              <input class="input" name="released" v-model="album.released" />
            </div>
            <br>
          </div>
        </div>
        <footer class="card-footer">

          <!-- Button element bound to an event handler with the @event syntax, calling a function defined below -->
          <button
            class="link card-footer-item cancel-button"
            @click="cancelAlbum()"
          >
            <i class="fas fa-undo"></i>
            <span>Cancel</span>
          </button>

          <!-- Another Button element bound to an event handler with the @event syntax, calling a function defined below -->
          <button class="link card-footer-item" @click="saveAlbum()">
            <i class="fas fa-save"></i>
            <span>Save</span>
          </button>
        </footer>
      </div>
    </div>
  </div>
</template>

<!-- Beginning of our javascript portion of the code (the meat) so to speak -->

<!-- Importing of our dataservice "client" the help aid with handling the REST services used by axios -->
<script>
import { dataService } from '../shared';

// The default data to be exported based on the relative data to AlbumDetail path, based on the object "id"
export default {
  name: 'AlbumDetail',
  props: {
    id: {
      type: Number,
      default: 0,
    },
  },

// The object to be returned, a whole album object, as defined in the array of the object
  data() {
    return {
      album: {},
    };
  },

  // Beginning the lifecycle hooks, grabbing an album based on id
  async created() {
    this.album = await dataService.getAlbum(this.id);
  },

  // Dynamic and mathmatical function that can allow single variable concatination if desires, not used here.
  computed: {
    albumName() {
      return this.album ? `${this.album.title}` : '';
    },
  },

  // Method calls to the buttons "Cancel and Save" and the action associated/path directed to after
  methods: {
    cancelAlbum() {
      this.$router.push({ name: 'Albums' });
    },
    async saveAlbum() {
      await dataService.updateAlbum(this.album);
      this.$router.push({ name: 'Albums' });
    },
  },
};
</script>
Album