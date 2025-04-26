//import * as axios from 'axios';

import { API } from './config';

import axios from "axios";

// Function designed to get all albums in from our albumsdb.json database, utilizing the API imported from the config.js file which is tied to the .env environment file
const getAlbums = async function() {
  try {
    const response = await axios.get(`${API}/albums`);
    let data = parseList(response);
    const albums = data;
    return albums;
  } catch (error) {
    console.error(error);
    return [];
  }
};

// Function designed to get a specific album, based on id, in from our albumsdb.json database, utilizing the API imported from the config.js file which is tied to the .env environment file
const getAlbum = async function(id) {
  try {
    console.log("Try to get album: " + id);
    const response = await axios.get(`${API}/albums/${id}`);
    let album = parseItem(response, 200);
    return album;
  } catch (error) {
    console.error(error);
    return null;
  }
};

// Function designed to put specific album details in place of existing, based on id, in from our albumsdb.json database, utilizing the API imported from the config.js file which is tied to the .env environment file
const updateAlbum = async function(album) {
  try {
    const response = await axios.put(`${API}/albums/${album.id}`, album);
    const updatedAlbum = parseItem(response, 200);
    return updatedAlbum;
  } catch (error) {
    console.error(error);
    return null;
  }
};

// Function designed to post a new album, with all included parameters, to our albumsdb.json database, utilizing the API imported from the config.js file which is tied to the .env environment file
const addAlbum = async function(album) {
  try {
    const response = await axios.post(`${API}/albums`, album);
    const addedAlbum = parseItem(response, 200);
    return addedAlbum;
  } catch (error) {
    console.error(error);
    return null;
  }
};

const parseList = response => {
  if (response.status !== 200) throw Error(response.message);
  if (!response.data) return [];
  let list = response.data;
  if (typeof list !== 'object') {
    list = [];
  }
  return list;
};

export const parseItem = (response, code) => {
  if (response.status !== code) throw Error(response.message);
  let item = response.data;
  if (typeof item !== 'object') {
    item = undefined;
  }
  return item;
};

export const dataService = {
  getAlbums,
  getAlbum,
  updateAlbum,
  addAlbum
};
