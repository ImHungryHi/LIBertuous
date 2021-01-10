/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package cdsdb;

/**
 *
 * @author ImHungryHi
 */
public class Column {
    private String name;
    private String type;
    private int typeSize;
    private int keyType;
    
    public Column() {
        this.name = "";
        this.type = "";
        this.typeSize = 0;
        this.keyType = KeyType.NORMAL;
    }
    
    public Column(String name, String type, int typeSize, int keyType) {
        this.name = name;
        this.type = type;
        this.typeSize = typeSize;
        this.keyType = keyType;
        
        if (keyType > 2) {
            this.keyType = KeyType.NORMAL;
        }
    }
    
    public String getName() {
        return this.name;
    }
    
    public void setName(String name) {
        this.name = name;
    }
    
    public String getType() {
        return this.type;
    }
    
    public void setType(String type) {
        this.type = type;
    }
    
    public int getTypeSize() {
        return this.typeSize;
    }
    
    public void setTypeSize(int typeSize) {
        this.typeSize = typeSize;
    }
    
    public int getKeyType() {
        return this.keyType;
    }
    
    public String getKeyTypeLong() {
        switch (this.keyType) {
            case KeyType.NORMAL: return "Normal";
            case KeyType.PRIMARY: return "Primary";
            case KeyType.FOREIGN: return "Foreign";
            default: return "Normal";
        }
    }
    
    public void setKeyType(int keyType) {
        if (keyType <= 2) {
            this.keyType = keyType;
        }
        else {
            this.keyType = KeyType.NORMAL;
        }
    }
    
    public void setKeyType(String keyType) {
        switch (keyType.toLowerCase()) {
            case "primary": this.keyType = KeyType.PRIMARY;
            break;
            case "foreign": this.keyType = KeyType.FOREIGN;
            break;
            default: this.keyType = KeyType.NORMAL;
            break;
        }
    }
}
