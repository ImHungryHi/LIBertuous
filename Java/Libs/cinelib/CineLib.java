/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package cinelib;

import cdsdb.DataBase;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

/**
 *
 * @author ImHungryHi
 */
public class CineLib {
    
    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        try {
            DataBase base = new DataBase();
            
            List<String> columns = Arrays.asList("Id", "Name", "LastName", "Age");
            
            System.out.println("dun dun duuuuun");
        }
        catch (Exception ex) {
            System.out.println(ex.getMessage());
        }
        finally {
            // tuck tail and run, meaning close all opened objects within the database
        }
    }
    
}
