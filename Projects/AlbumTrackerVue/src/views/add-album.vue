<!-- Add Album Page, creates a web based user input form, with differing field types, cancel and add buttons, as well as some dynamic vue items
     There is CSS helping the page layout look appealing, Data Validation for the user input to ensure quality data supplied. -->

<template>
    <div>
      <div class="section content-title-group">
        <h2 class="title">Add Album</h2>
        <div class="card">
          <header class="card-header">

            <!-- Dynamic header that displays the album title, as bound to the vue model, the value passed in utilizes interpolation syntax {{ }} -->
            <h2 class="card-header-title"><b>Title: {{ album.title }}</b></h2>
          </header>
          <div class="card-content">
            <div class="content">

                <!-- Basic textbox type input fields and binding to the vue model "dotted" reference style, this will be the field used for unique id validation -->
              <div class="field">
                <label class="label" for="id"><b>Album Id: </b></label>
                <input class="input" name="id" v-model="album.id" /><br>
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

              <!-- Placeholder for an error message, if needed, the value passed in utilizes interpolation syntax {{ }} -->
              <div class="Error">{{ message }}</div>
            </div>
          </div>
          <footer class="card-footer">

            <!-- Button element bound to an event handler with the @event syntax, calling a function defined below -->
            <button
              class="link card-footer-item cancel-button"
              @click="cancelAlbum()">
              <i class="fas fa-undo"></i>
              <span>Cancel Entry</span>

            <!-- Another Button element bound to an event handler with the @event syntax, calling a function defined below -->  
            </button>
            <button class="link card-footer-item" @click="addAlbum()">
              <i class="fas fa-save"></i>
              <span>Add Album</span>
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
  
  //   The default data that will be returned upon request, an album object with relevant variables, as well as a message variable to be used however needed (for this, errors)
  export default {
    data() {
      return {
        album: {
            id: '',                
            format: '',
            genre: '',
            artist: '',
            title: '',
            label: '',
            duration: '',
            cost: '',
            trackcount: '',
            released: '',
        },
        message: '',
      };
    },
       
    //  Beginning of the lifecycle hooks
    async created() {
    await this.loadAlbums();
    },
    
    // Beginning of the methods to be loaded during the lifecycle
    methods: {

        // Call to load in all albums into an array to be used during data validation
      async loadAlbums() {
        this.albums = [];
        this.message = 'Getting the albums, please be patient...';
  
        this.albums = await dataService.getAlbums();
        this.message = '';
      },

    //   "Cancel Button" function that just wipes all input fields.
      cancelAlbum() {
        this.album.id = '';
        this.album.format = '';
        this.album.genre = '';
        this.album.artist = '';
        this.album.title = '';
        this.album.label = '';
        this.album.duration = '';
        this.album.cost = '';
        this.album.trackcount = '';
        this.album.released = '';
        this.message = '';
      },

    //   "Add Button" function, along with the data validation to ensure fields are filled, and that the album id is unique.
    //    If validation fails, the .message div tag gets supplied content to be displayed on the page to inform user of errors and how to correct.
      async addAlbum() {
        if(this.album.id == '' || this.album.format == '' || this.album.genre == '' || this.album.artist == '' || this.album.title == '' || this.album.label == '' ||
        this.album.duration == '' || this.album.cost == '' || this.album.trackcount == '' || this.album.released == '')
        {
            this.message = 'Please Fill in all Form Fields for Valid Submission!';
            return;
        }

        // Simple while loop to ensure the album to be added has a unique id. Loops through the id variable of all items in the array of albums brought in to esnure uniqueness.
        // Error message supplied in the .message div tag if invalid
        let i = 0;
        let albumId = this.album.id;
            
        while(i < this.albums.length)
        {

            if(albumId.valueOf() == this.albums[i].id)
            {
                console.log("Duplicate ID while loop IF");
                this.message = 'Album ID already in use, please try another ID!'
                return;
            }

            i++
        }  

        // If all data validation passes, we push the new album object back to our dataservice "client" to add the new album to our albumsdb.json database
        await dataService.addAlbum(this.album);
        this.$router.push({ name: 'addconfirm' });  
      },
    },
  };
  </script>
  Album