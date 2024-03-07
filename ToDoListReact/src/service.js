import axios from 'axios';

// הגדרת כתובת ה-API כברירת מחדל
axios.defaults.baseURL = "http://localhost:5032";

export default {
  getTasks: async () => {
    try {
      const result = await axios.get("/items");
      return result.data;
    } catch (error) {
      console.error('Error fetching tasks:', error);
      throw error;
    }
  },

  addTask: async (name) => {
    try {
      const result = await axios.post("/items", { name });
      return result.data;
    } catch (error) {
      console.error('Error adding task:', error);


      throw error;
    }
  },

  setCompleted: async (id, isComplete) => {
    try {
      const result = await axios.put(`/items/${id}`, { isComplete });
      return result.data;
    } catch (error) {
      console.error('Error setting task completion status:', error);
      throw error;
    }
  },

  deleteTask: async (id) => {
    try {
      await axios.delete(`/items/${id}`);


      console.log(`Task with id ${id} deleted successfully.`);
    } catch (error) {
      console.error('Error deleting task:', error);


      throw error;
    }
  }
};
