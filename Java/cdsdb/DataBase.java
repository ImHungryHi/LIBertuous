/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package cdsdb;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.Dictionary;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

/**
 *
 * @author ImHungryHi
 */
public class DataBase {
    private final String namePrefix = "jdbc:sqlite:";
    private final String nameSuffix = ".db";
    private Connection connection;
    private String dbName;
    
    public DataBase() {
        this.dbName = "default";
        
        try {
            Class.forName("org.sqlite.JDBC");
            this.connection = DriverManager.getConnection(namePrefix + this.dbName + nameSuffix);
        }
        catch (Exception ex) {
            this.connection = null;
        }
    }
    
    public DataBase(String dbName) {
        // Stuff the class variables
        this.dbName = dbName;
        
        // Henceforth, create and fill the default variables
        try {
            Class.forName("org.sqlite.JDBC");
            this.connection = DriverManager.getConnection(namePrefix + this.dbName + nameSuffix);
        }
        catch (Exception ex) {
            this.connection = null;
        }
    }
    
    /**
     * 
     * @param tableName
     * @param columns consists of a 
     * @return 
     */
    public boolean Create(String tableName, Map<String, String>[] columns) {
        columns = new HashMap<String,String>();
    }
    
    public boolean Create(String tableName, List<String> columns) {
        String strCreate = "create table " + tableName + " (";
        
        if (columns.size() == 1) {
            strCreate += columns.get(0) + ");";
        }
        else {
            for (int x = 0;x < columns.size();x++) {
                if (x == (columns.size() - 1)) {
                    strCreate += columns.get(x) + ");";
                }
                else {
                    strCreate += columns.get(x) + ", ";
                }
            }
        }
        
        try {
            Statement statement = this.connection.createStatement();
            statement.executeUpdate("drop table if exists " + tableName + ";");
            statement.executeUpdate(strCreate);
            //statement.executeUpdate("create table " + tableName + " (Name, Occupation);");
        }
        catch (Exception ex) {
            return false;
        }
        
        return true;
    }
    
    public void SetDatabase(String dbName) {
        this.dbName = dbName;
        
        try {
            Class.forName("org.sqlite.JDBC");
            this.connection = DriverManager.getConnection(namePrefix + this.dbName + nameSuffix);
        }
        catch (Exception ex) {
            this.connection = null;
        }
    }
    
    public Map<String, String> Select(String table, int id) {
        Map<String, String> record = new HashMap<String, String>();
        String strSelect = "select * from " + table + " where id=" + id + ";";
        
        try {
            Statement statement = this.connection.createStatement();
            ResultSet result = statement.executeQuery(strSelect);
            int count = 1;
            
            while(result.next()) {
                if (count > 1) {
                    return null;
                }
                
                result.getInt("Id");
                
                count++;
            }
        }
        catch(Exception ex) {
            return null;
        }
        
        return record;
    }
    
    public Map<String,String> Select(String table, String key, String value) {
        return null;
    }
    
    public boolean Insert(String tableName, Map<String, String> values) {
        String strInsert = "";
        String strColumns = "(";
        String strValues = "(";
        Iterator entries = values.entrySet().iterator();
        
        while (entries.hasNext()) {
            Entry entry = (Entry) entries.next();
            
            if (!entries.hasNext())
            {
                strColumns += entry.getKey() + ")";
                strValues += entry.getValue() + ")";
            }
        }
        
        if (strColumns.equalsIgnoreCase("(") || strValues.equalsIgnoreCase("(")) {
            return false;
        }
        else {
            strInsert = "insert into " + tableName + strColumns + " values " + strValues + ";";
            
            try {
                Statement statement = this.connection.createStatement();
                statement.execute(strInsert);
            }
            catch (Exception ex) {
                return false;
            }
        }
        
        return true;
    }
    
    public boolean Update(String tableName, int id, String key, String value) {
        String strUpdate = "update " + tableName + " set " + key + "=" + value + " where id=" + id +";";
        
        try {
            Statement statement = this.connection.createStatement();
            statement.executeQuery(strUpdate);
        }
        catch (Exception ex) {
            return false;
        }
        
        return true;
    }
    
    public boolean Update(String tableName, Map<String, String> values) {
        String strUpdate = "";
        String strColumns = "(";
        String strValues = "(";
        Iterator entries = values.entrySet().iterator();
        
        while (entries.hasNext()) {
            Entry entry = (Entry) entries.next();
            
            if (!entries.hasNext())
            {
                strColumns += entry.getKey() + ")";
                strValues += entry.getValue() + ")";
            }
        }
        
        if (strColumns.equalsIgnoreCase("(") || strValues.equalsIgnoreCase("(")) {
            return false;
        }
        else {
            strUpdate = "insert into " + tableName + strColumns + " values " + strValues + ";";
            
            try {
                Statement statement = this.connection.createStatement();
                statement.execute(strUpdate);
            }
            catch (Exception ex) {
                return false;
            }
        }
        
        return true;
    }
    
    public boolean Close() {
        try {
            this.connection.close();
        }
        catch (Exception ex) {
            return false;
        }
        
        return true;
    }
}
